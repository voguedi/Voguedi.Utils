using System;
using System.Threading.Tasks;

namespace Voguedi.AsyncExecution
{
    public class AsyncExecutor : IAsyncExecutor
    {
        #region Private Enum
        
        enum AsyncExecutionType
        {
            Excute,
            Retry,
            RetryForever
        }

        #endregion

        #region Private Class

        class AsyncExecutionContext<TExecutedResult>
            where TExecutedResult : AsyncExecutedResult
        {
            #region Public Properties

            public AsyncExecutionType ExecutionType { get; set; }

            public Action<TExecutedResult> ResultAction { get; set; }

            public Action<Exception> ExceptionAction { get; set; }

            public Action<Exception, int> ExceptionActionForRetry { get; set; }

            public Action<int, int> RetryForeverAction { get; set; }

            public Action<int> RetryAction { get; set; }

            public int RetryTimes { get; set; }

            public int CurrentRetryTimes { get; set; }

            public int RetryInterval { get; set; }

            public int CurrentRetryInterval { get; set; }

            public Action<Exception, int, int> ExceptionActionForRetryForever { get; set; }

            #endregion
        }

        #endregion

        #region Private Methods

        void ContinuationAction<TExecutedResult>(Task<TExecutedResult> task, object state)
            where TExecutedResult : AsyncExecutedResult
        {
            if (state is AsyncExecutionContext<TExecutedResult> context)
            {
                if (task.Exception == null && !task.IsCanceled)
                {
                    var result = task.Result;

                    if (result != null && result.Succeeded)
                        ResultAction(context.ResultAction, result);
                    else if (result == null)
                        FailedAction(context, new Exception("未返回任何结果！"));
                    else if (!result.Succeeded)
                        FailedAction(context, result.Exception);
                }
                else if (task.Exception != null)
                    FailedAction(context, task.Exception);
                else if (task.IsCanceled)
                    FailedAction(context, new Exception("执行异常取消！"));
            }
        }

        void ResultAction<TExecutedResult>(Action<TExecutedResult> action, TExecutedResult result) where TExecutedResult : AsyncExecutedResult => action?.Invoke(result);

        void FailedAction<TExecutedResult>(AsyncExecutionContext<TExecutedResult> context, Exception exception)
            where TExecutedResult : AsyncExecutedResult
        {
            if (context.ExecutionType == AsyncExecutionType.Excute)
                ExceptionAction(context.ExceptionAction, exception);
            else if (context.ExecutionType == AsyncExecutionType.Retry)
                RetryAction(context.RetryAction, context.RetryTimes, context.CurrentRetryTimes, context.ExceptionActionForRetry, exception);
            else if (context.ExecutionType == AsyncExecutionType.RetryForever)
            {
                RetryForeverAction(
                    context.RetryForeverAction,
                    context.RetryTimes,
                    context.RetryInterval,
                    context.CurrentRetryInterval,
                    context.CurrentRetryTimes,
                    context.ExceptionActionForRetryForever,
                    exception);
            }
        }

        void ExceptionAction(Action<Exception> action, Exception exception) => action?.Invoke(exception);

        void RetryAction(Action<int> action, int retryTimes, int currentRetryTimes, Action<Exception, int> exceptionAction, Exception exception)
        {
            if (currentRetryTimes < retryTimes)
            {
                currentRetryTimes++;
                action?.Invoke(currentRetryTimes);
            }

            exceptionAction?.Invoke(exception, currentRetryTimes);
        }

        void RetryForeverAction(
            Action<int, int> action,
            int retryTimes,
            int retryInterval,
            int currentRetryInterval,
            int currentRetryTimes,
            Action<Exception, int, int> exceptionAction,
            Exception exception)
        {
            if (currentRetryTimes < retryTimes)
            {
                currentRetryTimes++;
                action?.Invoke(currentRetryInterval, currentRetryTimes);
            }
            else
            {
                currentRetryInterval += retryInterval;
                currentRetryTimes++;

                if (action != null)
                    Task.Factory.StartDelayed(currentRetryInterval, () => action(currentRetryInterval, currentRetryTimes));

                exceptionAction?.Invoke(exception, currentRetryInterval, currentRetryTimes);
            }
        }

        void ExecuteAndRetry<TExecutedResult>(
            Func<Task<TExecutedResult>> asyncAction,
            Action<TExecutedResult> resultAction,
            Action<Exception, int> exceptionAction,
            int retryTimes,
            int currentRetryTimes)
            where TExecutedResult : AsyncExecutedResult
        {
            ExecuteAndRetry(
                asyncAction,
                resultAction,
                exceptionAction,
                c => ExecuteAndRetry(asyncAction, resultAction, exceptionAction, retryTimes, c),
                retryTimes,
                currentRetryTimes);
        }

        void ExecuteAndRetry<TExecutedResult>(
            Func<Task<TExecutedResult>> asyncAction,
            Action<TExecutedResult> resultAction,
            Action<Exception, int> exceptionAction,
            Action<int> retryAction,
            int retryTimes,
            int currentRetryTimes)
            where TExecutedResult : AsyncExecutedResult
        {
            try
            {
                asyncAction().ContinueWith(
                    ContinuationAction,
                    new AsyncExecutionContext<TExecutedResult>
                    {
                        CurrentRetryTimes = currentRetryTimes,
                        ExceptionActionForRetry = exceptionAction,
                        ResultAction = resultAction,
                        RetryAction = retryAction,
                        RetryTimes = retryTimes,
                        ExecutionType = AsyncExecutionType.Retry
                    });
            }
            catch (Exception ex)
            {
                RetryAction(retryAction, retryTimes, currentRetryTimes, exceptionAction, ex);
            }
        }

        void ExecuteAndRetryForever<TExecutedResult>(
            Func<Task<TExecutedResult>> asyncAction,
            Action<TExecutedResult> resultAction,
            Action<Exception, int, int> exceptionAction,
            int retryTimes,
            int retryInterval,
            int currentRetryInterval,
            int currentRetryTimes)
            where TExecutedResult : AsyncExecutedResult
        {
            ExecuteAndRetryForever(
                asyncAction,
                resultAction,
                exceptionAction,
                (i, t) => ExecuteAndRetryForever(asyncAction, resultAction, exceptionAction, retryTimes, retryInterval, i, t),
                retryTimes,
                retryInterval,
                currentRetryInterval,
                currentRetryTimes);
        }

        void ExecuteAndRetryForever<TExecutedResult>(
            Func<Task<TExecutedResult>> asyncAction,
            Action<TExecutedResult> resultAction,
            Action<Exception, int, int> exceptionAction,
            Action<int, int> retryForeverAction,
            int retryTimes,
            int retryInterval,
            int currentRetryInterval,
            int currentRetryTimes)
            where TExecutedResult : AsyncExecutedResult
        {
            try
            {
                asyncAction().ContinueWith(
                    ContinuationAction,
                    new AsyncExecutionContext<TExecutedResult>
                    {
                        CurrentRetryInterval = currentRetryInterval,
                        CurrentRetryTimes = currentRetryTimes,
                        ExceptionActionForRetryForever = exceptionAction,
                        ExecutionType = AsyncExecutionType.RetryForever,
                        ResultAction = resultAction,
                        RetryForeverAction = retryForeverAction,
                        RetryInterval = retryInterval,
                        RetryTimes = retryTimes
                    });
            }
            catch (Exception ex)
            {
                RetryForeverAction(retryForeverAction, retryTimes, retryInterval, currentRetryInterval, currentRetryTimes, exceptionAction, ex);
            }
        }

        #endregion
        
        #region IAsyncExecutor

        public void Execute<TExecutedResult>(Func<Task<TExecutedResult>> asyncAction, Action<TExecutedResult> resultAction = null, Action<Exception> exceptionAction = null)
            where TExecutedResult : AsyncExecutedResult
        {
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            try
            {
                asyncAction().ContinueWith(
                    ContinuationAction,
                    new AsyncExecutionContext<TExecutedResult>
                    {
                        ExceptionAction = exceptionAction,
                        ExecutionType = AsyncExecutionType.Excute,
                        ResultAction = resultAction
                    });
            }
            catch (Exception ex)
            {
                ExceptionAction(exceptionAction, ex);
            }
        }

        public void ExecuteAndRetry<TExecutedResult>(
            Func<Task<TExecutedResult>> asyncAction,
            Action<TExecutedResult> resultAction = null,
            Action<Exception, int> exceptionAction = null,
            int retryTimes = 3)
            where TExecutedResult : AsyncExecutedResult
        {
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            if (retryTimes < 0)
                throw new ArgumentOutOfRangeException(nameof(retryTimes));

            ExecuteAndRetry(asyncAction, resultAction, exceptionAction, retryTimes, 0);
        }

        public void ExecuteAndRetryForever<TExecutedResult>(
            Func<Task<TExecutedResult>> asyncAction,
            Action<TExecutedResult> resultAction = null,
            Action<Exception, int, int> exceptionAction = null,
            int retryTimes = 3,
            int retryInterval = 1000)
            where TExecutedResult : AsyncExecutedResult
        {
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            if (retryTimes < 0)
                throw new ArgumentOutOfRangeException(nameof(retryTimes));

            if (retryInterval < 0)
                throw new ArgumentOutOfRangeException(nameof(retryInterval));

            ExecuteAndRetryForever(asyncAction, resultAction, exceptionAction, retryTimes, retryInterval, 0, 0);
        }

        #endregion
    }
}
