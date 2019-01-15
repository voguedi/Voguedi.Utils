using System;
using Autofac;
using Autofac.Core;
using Voguedi.DependencyInjection.Autofac;

namespace Voguedi.DependencyInjection
{
    public static class IocManagerExtensions
    {
        #region Public Methods

        public static void UseAutofacObjectContainer(this IIocManager iocManager)
        {
            if (iocManager == null)
                throw new ArgumentNullException(nameof(iocManager));

            iocManager.SetObjectContainer(new AutofacObjectContainer());
        }

        public static void RegisterAutofacContainerBuilder(this IIocManager iocManager, Action<ContainerBuilder> containerBuilderAction)
        {
            if (iocManager == null)
                throw new ArgumentNullException(nameof(iocManager));

            iocManager.ObjectContainer.ToAutofacObjectContainer().Register(containerBuilderAction);
        }

        public static void RegisterAutofacModules(this IIocManager iocManager, params IModule[] modules)
        {
            if (iocManager == null)
                throw new ArgumentNullException(nameof(iocManager));

            iocManager.ObjectContainer.ToAutofacObjectContainer().RegisterModules(modules);
        }

        #endregion
    }
}
