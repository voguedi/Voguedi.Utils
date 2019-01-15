using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Voguedi.DependencyInjection
{
    public interface IIocManager : IDisposable
    {
        #region Properties

        IObjectContainer ObjectContainer { get; }

        #endregion

        #region Methods

        void SetObjectContainer(IObjectContainer objectContainer);

        IServiceProvider Build();

        void Register(IServiceCollection services);

        void Register(Action<IServiceCollection> servicesAction);

        void RegisterAssemblies(params Assembly[] assemblies);

        void Register(Type serviceType, Lifetime lifetime = Lifetime.Singleton);

        void RegisterNamed(Type serviceType, string serviceName, Lifetime lifetime = Lifetime.Singleton);

        void Register<TService>(Lifetime lifetime = Lifetime.Singleton) where TService : class;

        void RegisterNamed<TService>(string serviceName, Lifetime lifetime = Lifetime.Singleton) where TService : class;

        void Register(Type serviceType, Type implementationType, Lifetime lifetime = Lifetime.Singleton);

        void RegisterNamed(Type serviceType, Type implementationType, string serviceName, Lifetime lifetime = Lifetime.Singleton);

        void Register<TService, TImplementation>(Lifetime lifetime = Lifetime.Singleton)
            where TService : class
            where TImplementation : class, TService;

        void RegisterNamed<TService, TImplementation>(string serviceName, Lifetime lifetime = Lifetime.Singleton)
            where TService : class
            where TImplementation : class, TService;

        void RegisterTypes(Type serviceType, IReadOnlyList<Type> implementationTypes, Lifetime lifetime = Lifetime.Singleton);

        void RegisterTypesNamed(Type serviceType, IReadOnlyList<Type> implementationTypes, string serviceName, Lifetime lifetime = Lifetime.Singleton);

        void RegisterInstance(Type serviceType, object implementation);

        void RegisterInstanceNamed(Type serviceType, object implementation, string serviceName);

        void RegisterInstance<TService, TImplementation>(TImplementation implementation)
            where TService : class
            where TImplementation : class, TService;

        void RegisterInstanceNamed<TService, TImplementation>(TImplementation implementation, string serviceName)
            where TService : class
            where TImplementation : class, TService;

        object Resolve(Type serviceType);

        object ResolveNamed(Type serviceType, string serviceName);

        TService Resolve<TService>() where TService : class;

        TService ResolveNamed<TService>(string serviceName) where TService : class;

        IReadOnlyList<object> ResolveAll(Type serviceType);

        IReadOnlyList<object> ResolveAllNamed(Type serviceType, string serviceName);

        IReadOnlyList<TService> ResolveAll<TService>() where TService : class;

        IReadOnlyList<TService> ResolveAllNamed<TService>(string serviceName) where TService : class;

        bool TryResolve(Type serviceType, out object service);

        bool TryResolveNamed(Type serviceType, string serviceName, out object service);

        bool TryResolve<TService>(out TService service) where TService : class;

        bool TryResolveNamed<TService>(string serviceName, out TService service) where TService : class;

        bool TryResolveAll(Type serviceType, out IReadOnlyList<object> services);

        bool TryResolveAllNamed(Type serviceType, string serviceName, out IReadOnlyList<object> services);

        bool TryResolveAll<TService>(out IReadOnlyList<TService> services) where TService : class;

        bool TryResolveAllNamed<TService>(string serviceName, out IReadOnlyList<TService> services) where TService : class;

        IScopedResolver CreateScope();

        #endregion
    }
}
