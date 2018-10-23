using System;

namespace Voguedi.Utils.AsyncExecution
{
    public class AsyncExecutionResult
    {
        #region Public Properties

        public bool Succeeded { get; }

        public string ErrorMessage { get; }

        public Exception Exception { get; }

        public static AsyncExecutionResult Success => new AsyncExecutionResult();

        #endregion

        #region Ctors

        protected AsyncExecutionResult() => Succeeded = true;

        protected AsyncExecutionResult(Exception exception)
        {
            Succeeded = false;
            Exception = exception;
            ErrorMessage = exception.ToString();
        }

        #endregion

        #region Public Methods

        public static AsyncExecutionResult Failed(Exception exception) => new AsyncExecutionResult(exception);

        #endregion
    }

    public class AsyncExecutionResult<TData> : AsyncExecutionResult
    {
        #region Public Properties

        public TData Data { get; }

        #endregion

        #region Ctors

        protected AsyncExecutionResult(TData data) : base() => Data = data;

        protected AsyncExecutionResult(Exception exception) : base(exception) => Data = default(TData);

        protected AsyncExecutionResult(Exception exception, TData data) : base(exception) => Data = data;

        #endregion

        #region Public Methods

        public new static AsyncExecutionResult<TData> Success(TData data) => new AsyncExecutionResult<TData>(data);

        public new static AsyncExecutionResult<TData> Failed(Exception exception) => new AsyncExecutionResult<TData>(exception);

        public static AsyncExecutionResult<TData> Failed(Exception exception, TData data) => new AsyncExecutionResult<TData>(exception, data);

        #endregion
    }
}
