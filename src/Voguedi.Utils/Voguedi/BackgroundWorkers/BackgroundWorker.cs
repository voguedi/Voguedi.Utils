using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Voguedi.BackgroundWorkers
{
    class BackgroundWorker : IBackgroundWorker
    {
        #region Private Class

        class BackgroundWorkerContext
        {
            #region Public Properties

            public string Key { get; set; }

            public Action Action { get; set; }

            public Timer Timer { get; set; }

            public int DueTime { get; set; }

            public int Period { get; set; }

            public bool Started { get; set; }

            #endregion
        }

        #endregion

        #region Private Fields

        readonly ILogger logger;
        readonly object syncLock = new object();
        readonly ConcurrentDictionary<string, BackgroundWorkerContext> contextMapping = new ConcurrentDictionary<string, BackgroundWorkerContext>();

        #endregion

        #region Ctors

        public BackgroundWorker(ILogger<BackgroundWorker> logger) => this.logger = logger;

        #endregion

        #region Private Methods

        void Callback(object state)
        {
            var key = state.ToString();

            if (contextMapping.TryGetValue(key, out var context))
            {
                try
                {
                    if (context.Started)
                    {
                        context.Timer.Change(Timeout.Infinite, Timeout.Infinite);
                        context.Action?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"当前状态异常！ [Key = {context.Key}, DueTime = {context.DueTime}, Period = {context.Period}]");
                }
                finally
                {
                    try
                    {
                        if (context.Started)
                            context.Timer.Change(context.Period, context.Period);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"当前状态异常！ [Key = {context.Key}, DueTime = {context.DueTime}, Period = {context.Period}]");
                    }
                }
            }
        }

        #endregion

        #region IBackgroundWorker

        public void Start(string key, Action action, int dueTime, int period)
        {
            lock (syncLock)
            {
                if (!contextMapping.ContainsKey(key))
                {
                    var timer = new Timer(Callback, key, Timeout.Infinite, Timeout.Infinite);
                    contextMapping.TryAdd(
                        key,
                        new BackgroundWorkerContext
                        {
                            Action = action,
                            DueTime = dueTime,
                            Key = key,
                            Period = period,
                            Started = true,
                            Timer = timer
                        });
                    timer.Change(dueTime, period);
                }
            }
        }

        public void Stop(string key)
        {
            lock (syncLock)
            {
                if (contextMapping.TryGetValue(key, out var context))
                {
                    context.Started = false;
                    context.Timer.Dispose();
                    contextMapping.TryRemove(key);
                }
            }
        }

        #endregion
    }
}
