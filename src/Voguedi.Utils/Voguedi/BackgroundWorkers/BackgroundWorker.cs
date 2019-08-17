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

            public string Id { get; set; }

            public Action Action { get; set; }

            public Timer Timer { get; set; }

            public int DueTime { get; set; }

            public int Period { get; set; }

            public bool Started { get; set; }

            #endregion
        }

        #endregion

        #region Private Fields

        static readonly object syncLock = new object();
        readonly ILogger logger;
        readonly ConcurrentDictionary<string, BackgroundWorkerContext> contextMapping;

        #endregion

        #region Ctors

        public BackgroundWorker(ILogger<BackgroundWorker> logger)
        {
            this.logger = logger;
            contextMapping = new ConcurrentDictionary<string, BackgroundWorkerContext>();
        }

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
                    logger.LogError(ex, $"Background worker error! [Id = {context.Id}, DueTime = {context.DueTime}, Period = {context.Period}]");
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
                        logger.LogError(ex, $"Background worker error! [Id = {context.Id}, DueTime = {context.DueTime}, Period = {context.Period}]");
                    }
                }
            }
        }

        #endregion

        #region IBackgroundWorker

        public void Start(string id, Action action, int dueTime, int period)
        {
            lock (syncLock)
            {
                if (!contextMapping.ContainsKey(id))
                {
                    var timer = new Timer(Callback, id, Timeout.Infinite, Timeout.Infinite);
                    contextMapping.TryAdd(
                        id,
                        new BackgroundWorkerContext
                        {
                            Action = action,
                            DueTime = dueTime,
                            Id = id,
                            Period = period,
                            Started = true,
                            Timer = timer
                        });
                    timer.Change(dueTime, period);
                }
            }
        }

        public void Stop(string id)
        {
            lock (syncLock)
            {
                if (contextMapping.TryGetValue(id, out var context))
                {
                    context.Started = false;
                    context.Timer.Dispose();
                    contextMapping.TryRemove(id);
                }
            }
        }

        #endregion
    }
}
