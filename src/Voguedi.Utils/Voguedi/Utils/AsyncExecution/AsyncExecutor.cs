using System;
using System.Threading.Tasks;

namespace Voguedi.Utils.AsyncExecution
{
    public class AsyncExecutor : IAsyncExecutor
    {
        #region Private Class

        class AsyncExecutionContext<TExecutionResult>
            where TExecutionResult : AsyncExecutionResult
        {
            #region Public Properties

            public Action<TExecutionResult> ResultAction { get; set; }

            public Action<Exception> ExceptionAction { get; set; }

            #endregion
        }

        #endregion

        #region Private Methods

        void Continue<TExecutionResult>(Task<TExecutionResult> task, object state)
            where TExecutionResult : AsyncExecutionResult
        {
            var context = state as AsyncExecutionContext<TExecutionResult>;

            if (task.Exception == null && !task.IsCanceled)
            {
                var result = task.Result;

                if (result != null && result.Succeeded)
                    context.ResultAction(result);
                else if (result == null)
                    context.ExceptionAction(new Exception("无返回结果！"));
                else if (!result.Succeeded)
                    context.ExceptionAction(result.Exception);
            }
            else if (task.Exception != null)
                context.ExceptionAction(task.Exception);
            else if (task.IsCanceled)
                context.ExceptionAction(new Exception("执行被取消！"));
        }

        #endregion

        #region IAsyncExecutor

        public void Execute<TExecutionResult>(Func<Task<TExecutionResult>> asyncAction, Action<TExecutionResult> resultAction, Action<Exception> exceptionAction)
            where TExecutionResult : AsyncExecutionResult
        {
            var context = new AsyncExecutionContext<TExecutionResult>
            {
                ExceptionAction = exceptionAction,
                ResultAction = resultAction
            };

            try
            {
                asyncAction().ContinueWith(Continue, context);
            }
            catch (Exception ex)
            {
                exceptionAction(ex);
            }
        }

        #endregion
    }
}
