using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Voguedi.DisposableObjects;

namespace Voguedi.DependencyInjection
{
    public class IocManager : DisposableObject, IIocManager
    {
        #region Private Fields

        bool disposed;

        #endregion

        #region Public Properties

        public static IocManager Instance { get; } = new IocManager();

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    ObjectContainer.Dispose();

                disposed = true;
            }
        }

        #endregion

        #region IIocManager

        public IObjectContainer ObjectContainer { get; private set; } = new ObjectContainer();

        public void SetObjectContainer(IObjectContainer objectContainer) => ObjectContainer = objectContainer;

        public IServiceProvider Build() => ObjectContainer.Build();

        public void Register(IServiceCollection services) => ObjectContainer.Register(services);

        public IScopedResolver CreateScope() => ObjectContainer.CreateScope();

        public void Register(Type serviceType, Lifetime lifetime = Lifetime.Singleton) => ObjectContainer.Register(serviceType, lifetime);

        public void Register<TService>(Lifetime lifetime = Lifetime.Singleton) where TService : class => ObjectContainer.Register<TService>(lifetime);

        public void Register(Type serviceType, Type implementationType, Lifetime lifetime = Lifetime.Singleton) => ObjectContainer.Register(serviceType, implementationType, lifetime);

        public void Register<TService, TImplementation>(Lifetime lifetime = Lifetime.Singleton)
            where TService : class
            where TImplementation : class, TService
            => ObjectContainer.Register<TService, TImplementation>(lifetime);

        public void RegisterNamed(Type serviceType, string serviceName, Lifetime lifetime = Lifetime.Singleton) => ObjectContainer.RegisterNamed(serviceType, serviceName, lifetime);

        public void RegisterNamed<TService>(string serviceName, Lifetime lifetime = Lifetime.Singleton) where TService : class => ObjectContainer.RegisterNamed<TService>(serviceName, lifetime);

        public void RegisterNamed(Type serviceType, Type implementationType, string serviceName, Lifetime lifetime = Lifetime.Singleton)
            => ObjectContainer.RegisterNamed(serviceType, implementationType, serviceName, lifetime);

        public void RegisterNamed<TService, TImplementation>(string serviceName, Lifetime lifetime = Lifetime.Singleton)
            where TService : class
            where TImplementation : class, TService
            => ObjectContainer.RegisterNamed<TService, TImplementation>(serviceName, lifetime);

        public void RegisterTypes(Type serviceType, IReadOnlyList<Type> implementationTypes, Lifetime lifetime = Lifetime.Singleton)
            => ObjectContainer.RegisterTypes(serviceType, implementationTypes, lifetime);

        public void RegisterTypesNamed(Type serviceType, IReadOnlyList<Type> implementationTypes, string serviceName, Lifetime lifetime = Lifetime.Singleton)
            => ObjectContainer.RegisterTypesNamed(serviceType, implementationTypes, serviceName, lifetime);

        public object Resolve(Type serviceType) => ObjectContainer.Resolve(serviceType);

        public TService Resolve<TService>() where TService : class => ObjectContainer.Resolve<TService>();

        public IReadOnlyList<object> ResolveAll(Type serviceType) => ObjectContainer.ResolveAll(serviceType);

        public IReadOnlyList<TService> ResolveAll<TService>() where TService : class => ObjectContainer.ResolveAll<TService>();

        public IReadOnlyList<object> ResolveAllNamed(Type serviceType, string serviceName) => ObjectContainer.ResolveAllNamed(serviceType, serviceName);

        public IReadOnlyList<TService> ResolveAllNamed<TService>(string serviceName) where TService : class => ObjectContainer.ResolveAllNamed<TService>(serviceName);

        public object ResolveNamed(Type serviceType, string serviceName) => ObjectContainer.ResolveNamed(serviceType, serviceName);

        public TService ResolveNamed<TService>(string serviceName) where TService : class => ObjectContainer.ResolveNamed<TService>(serviceName);

        public bool TryResolve(Type serviceType, out object service) => ObjectContainer.TryResolve(serviceType, out service);

        public bool TryResolve<TService>(out TService service) where TService : class => ObjectContainer.TryResolve(out service);

        public bool TryResolveAll(Type serviceType, out IReadOnlyList<object> services) => ObjectContainer.TryResolveAll(serviceType, out services);

        public bool TryResolveAll<TService>(out IReadOnlyList<TService> services) where TService : class => ObjectContainer.TryResolveAll(out services);

        public bool TryResolveAllNamed(Type serviceType, string serviceName, out IReadOnlyList<object> services) => ObjectContainer.TryResolveAllNamed(serviceType, serviceName, out services);

        public bool TryResolveAllNamed<TService>(string serviceName, out IReadOnlyList<TService> services)
            where TService : class
            => ObjectContainer.TryResolveAllNamed(serviceName, out services);

        public bool TryResolveNamed(Type serviceType, string serviceName, out object service) => ObjectContainer.TryResolveNamed(serviceType, serviceName, out service);

        public bool TryResolveNamed<TService>(string serviceName, out TService service) where TService : class => ObjectContainer.TryResolveNamed(serviceName, out service);

        #endregion
    }
}
