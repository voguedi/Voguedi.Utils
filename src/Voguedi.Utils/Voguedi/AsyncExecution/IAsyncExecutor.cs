using System;
using System.Threading.Tasks;
using Voguedi.DependencyInjection;

namespace Voguedi.AsyncExecution
{
    public interface IAsyncExecutor : ISingletonDependency
    {
        #region Methods

        void Execute<TExecutedResult>(Func<Task<TExecutedResult>> asyncAction, Action<TExecutedResult> resultAction = null, Action<Exception> exceptionAction = null)
            where TExecutedResult : AsyncExecutedResult;

        void ExecuteAndRetry<TExecutedResult>(
            Func<Task<TExecutedResult>> asyncAction,
            Action<TExecutedResult> resultAction = null,
            Action<Exception, int> exceptionAction = null,
            int retryTimes = 3)
            where TExecutedResult : AsyncExecutedResult;

        void ExecuteAndRetryForever<TExecutedResult>(
            Func<Task<TExecutedResult>> asyncAction,
            Action<TExecutedResult> resultAction,
            Action<Exception, int, int> exceptionAction,
            int retryInterval = 1000)
            where TExecutedResult : AsyncExecutedResult;

        #endregion
    }
}
