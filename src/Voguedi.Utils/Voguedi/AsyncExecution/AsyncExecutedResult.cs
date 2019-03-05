using System;

namespace Voguedi.AsyncExecution
{
    public class AsyncExecutedResult
    {
        #region Ctors

        protected AsyncExecutedResult() => Succeeded = true;

        protected AsyncExecutedResult(Exception exception)
        {
            Succeeded = false;
            Exception = exception;
            ErrorMessage = exception.ToString();
        }

        #endregion

        #region Public Properties

        public bool Succeeded { get; }

        public string ErrorMessage { get; }

        public Exception Exception { get; }

        public static AsyncExecutedResult Success => new AsyncExecutedResult();

        #endregion

        #region Public Methods

        public static AsyncExecutedResult Failed(Exception exception) => new AsyncExecutedResult(exception);

        #endregion
    }

    public class AsyncExecutedResult<TData> : AsyncExecutedResult
    {
        #region Ctors

        protected AsyncExecutedResult(TData data) : base() => Data = data;

        protected AsyncExecutedResult(Exception exception) : base(exception) => Data = default(TData);

        protected AsyncExecutedResult(Exception exception, TData data) : base(exception) => Data = data;

        #endregion

        #region Public Properties

        public TData Data { get; }

        #endregion

        #region Public Methods

        public new static AsyncExecutedResult<TData> Success(TData data) => new AsyncExecutedResult<TData>(data);

        public new static AsyncExecutedResult<TData> Failed(Exception exception) => new AsyncExecutedResult<TData>(exception);

        public static AsyncExecutedResult<TData> Failed(Exception exception, TData data) => new AsyncExecutedResult<TData>(exception, data);

        #endregion
    }
}
