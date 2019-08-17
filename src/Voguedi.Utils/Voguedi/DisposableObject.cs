using System;

namespace Voguedi
{
    public abstract class DisposableObject : IDisposable
    {
        #region Finalized Ctors

        ~DisposableObject() => Dispose(false);

        #endregion

        #region Protected Methods

        protected abstract void Dispose(bool disposing);

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
