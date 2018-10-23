using System;
using System.Threading.Tasks;
using Voguedi.Utils.DependencyInjection;

namespace Voguedi.Utils.AsyncExecution
{
    public interface IAsyncExecutor : ISingletonDependency
    {
        #region Methods

        void Execute<TExecutionResult>(Func<Task<TExecutionResult>> asyncAction, Action<TExecutionResult> resultAction, Action<Exception> exceptionAction)
            where TExecutionResult : AsyncExecutionResult;

        #endregion
    }
}
