using System;
using Voguedi.DependencyInjection;

namespace Voguedi.BackgroundWorkers
{
    public interface IBackgroundWorker : ISingletonDependency
    {
        #region Methods

        void Start(string key, Action action, int dueTime, int period);

        void Stop(string key);

        #endregion
    }
}
