using System;
using Autofac;
using Autofac.Core;

namespace Voguedi.DependencyInjection.Autofac
{
    public interface IAutofacObjectContainer : IObjectContainer
    {
        #region Methods

        void RegisterContainerBuilder(Action<ContainerBuilder> containerBuilderAction);

        void RegisterModules(params IModule[] modules);

        #endregion
    }
}
