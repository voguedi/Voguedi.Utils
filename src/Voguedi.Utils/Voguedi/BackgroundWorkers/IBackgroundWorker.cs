using System;

namespace Voguedi.BackgroundWorkers
{
    public interface IBackgroundWorker
    {
        #region Methods

        void Start(string id, Action action, int dueTime, int period);

        void Stop(string id);

        #endregion
    }
}
