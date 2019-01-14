using System;
using Autofac.Core;
using Voguedi.DependencyInjection.Autofac;

namespace Voguedi.DependencyInjection
{
    public static class ObjectContainerExtensions
    {
        #region Public Methods

        public static IAutofacObjectContainer ToAutofacObjectContainer(this IObjectContainer objectContainer)
        {
            if (objectContainer == null)
                throw new ArgumentNullException(nameof(objectContainer));

            if (objectContainer is IAutofacObjectContainer autofacObjectContainer)
                return autofacObjectContainer;

            throw new ArgumentException(nameof(objectContainer));
        }

        public static void RegisterAutofacModules(this IObjectContainer objectContainer, params IModule[] modules)
        {
            if (objectContainer == null)
                throw new ArgumentNullException(nameof(objectContainer));

            objectContainer.ToAutofacObjectContainer().RegisterModules(modules);
        }

        #endregion
    }
}
