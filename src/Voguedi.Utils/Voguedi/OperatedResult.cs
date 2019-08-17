using System;

namespace Voguedi
{
    public class OperatedResult
    {
        #region Public Fields

        public static readonly OperatedResult Success = new OperatedResult();

        #endregion

        #region Ctors

        protected OperatedResult() { }

        protected OperatedResult(Exception exception)
        {
            Succeeded = false;
            Exception = exception;
        }

        #endregion

        #region Public Properties

        public bool Succeeded { get; } = true;

        public Exception Exception { get; }

        public string ErrorMessage => Exception?.ToString();

        #endregion

        #region Public Methods

        public static OperatedResult Failed(Exception exception) => new OperatedResult(exception);

        #endregion
    }

    public class OperatedResult<TValue> : OperatedResult
    {
        #region Ctors

        protected OperatedResult(TValue value) : base() => Value = value;

        protected OperatedResult(Exception exception) : base(exception) => Value = default;

        protected OperatedResult(Exception exception, TValue value) : base(exception) => Value = value;

        #endregion

        #region Public Properties

        public TValue Value { get; }

        #endregion

        #region Public Methods

        public new static OperatedResult<TValue> Success(TValue value) => new OperatedResult<TValue>(value);

        public new static OperatedResult<TValue> Failed(Exception exception) => new OperatedResult<TValue>(exception);

        public static OperatedResult<TValue> Failed(Exception exception, TValue value) => new OperatedResult<TValue>(exception, value);

        #endregion
    }
}
