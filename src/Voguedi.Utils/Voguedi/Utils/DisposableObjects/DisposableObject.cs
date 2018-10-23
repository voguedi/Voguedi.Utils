using System;

namespace Voguedi.Utils.DisposableObjects
{
    public abstract class DisposableObject : IDisposable
    {
        #region Finalization Ctors

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
