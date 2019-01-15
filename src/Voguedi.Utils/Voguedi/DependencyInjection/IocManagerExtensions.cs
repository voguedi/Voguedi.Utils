using System;

namespace Voguedi.DependencyInjection
{
    public static class IocManagerExtensions
    {
        #region Public Methods

        public static void UseDefaultObjectContainer(this IIocManager iocManager)
        {
            if (iocManager == null)
                throw new ArgumentNullException(nameof(iocManager));

            iocManager.SetObjectContainer(new ObjectContainer());
        }

        #endregion
    }
}
