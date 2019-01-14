using System;

namespace Voguedi.BackgroundWorkers
{
    public interface IBackgroundWorker
    {
        #region Methods

        void Start(string key, Action action, int dueTime, int period);

        void Stop(string key);

        #endregion
    }
}
