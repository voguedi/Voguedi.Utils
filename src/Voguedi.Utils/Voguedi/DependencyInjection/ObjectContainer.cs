using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.DisposableObjects;
using Voguedi.Reflection;

namespace Voguedi.DependencyInjection
{
    public class ObjectContainer : DisposableObject, IObjectContainer
    {
        #region Private Fields

        readonly IServiceCollection services;

        #endregion

        #region Protected Fileds

        protected IServiceProvider ServiceProvider;
        protected readonly ITypeFinder TypeFinder = new TypeFinder();

        #endregion

        #region Ctors

        public ObjectContainer() => services = new ServiceCollection();

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing) { }

        #endregion

        #region IObjectContainer

        public virtual IServiceProvider Build() => ServiceProvider = services.BuildServiceProvider();

        public virtual void Register(IServiceCollection services)
        {
            if (services?.Count > 0)
                this.services.TryAdd(services);
        }

        public virtual void Register(Type serviceType, Lifetime lifetime = Lifetime.Singleton) => RegisterNamed(serviceType, null, lifetime);

        public virtual void RegisterNamed(Type serviceType, string serviceName, Lifetime lifetime = Lifetime.Singleton) => RegisterNamed(serviceType, serviceType, serviceName, lifetime);

        public virtual void Register<TService>(Lifetime lifetime = Lifetime.Singleton) where TService : class => RegisterNamed<TService>(null, lifetime);

        public virtual void RegisterNamed<TService>(string serviceName, Lifetime lifetime = Lifetime.Singleton)
            where TService : class
            => RegisterNamed(typeof(TService), serviceName, lifetime);

        public virtual void Register(Type serviceType, Type implementationType, Lifetime lifetime = Lifetime.Singleton) => RegisterNamed(serviceType, implementationType, null, lifetime);

        public virtual void RegisterNamed(Type serviceType, Type implementationType, string serviceName, Lifetime lifetime = Lifetime.Singleton)
        {
            if (lifetime == Lifetime.Scoped)
                services.TryAdd(ServiceDescriptor.Scoped(serviceType, implementationType));
            else if (lifetime == Lifetime.Singleton)
                services.TryAdd(ServiceDescriptor.Singleton(serviceType, implementationType));
            else
                services.TryAdd(ServiceDescriptor.Transient(serviceType, implementationType));
        }

        public virtual void Register<TService, TImplementation>(Lifetime lifetime = Lifetime.Singleton)
            where TService : class
            where TImplementation : class, TService
            => RegisterNamed<TService, TImplementation>(null, lifetime);

        public virtual void RegisterNamed<TService, TImplementation>(string serviceName, Lifetime lifetime = Lifetime.Singleton)
            where TService : class
            where TImplementation : class, TService
            => RegisterNamed(typeof(TService), typeof(TImplementation), serviceName, lifetime);

        public virtual void RegisterTypes(Type serviceType, IReadOnlyList<Type> implementationTypes, Lifetime lifetime = Lifetime.Singleton)
            => RegisterTypesNamed(serviceType, implementationTypes, null, lifetime);

        public virtual void RegisterTypesNamed(Type serviceType, IReadOnlyList<Type> implementationTypes, string serviceName, Lifetime lifetime = Lifetime.Singleton)
        {
            if (lifetime == Lifetime.Scoped)
            {
                foreach (var implementationType in implementationTypes)
                    services.TryAddEnumerable(ServiceDescriptor.Scoped(serviceType, implementationType));
            }
            else if (lifetime == Lifetime.Singleton)
            {
                foreach (var implementationType in implementationTypes)
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(serviceType, implementationType));
            }
            else
            {
                foreach (var implementationType in implementationTypes)
                    services.TryAddEnumerable(ServiceDescriptor.Transient(serviceType, implementationType));
            }
        }

        public virtual object Resolve(Type serviceType) => ResolveNamed(serviceType, null);

        public virtual object ResolveNamed(Type serviceType, string serviceName) => ServiceProvider.GetService(serviceType);

        public virtual TService Resolve<TService>() where TService : class => ResolveNamed<TService>(null);

        public virtual TService ResolveNamed<TService>(string serviceName) where TService : class => ServiceProvider.GetService<TService>();

        public virtual IReadOnlyList<object> ResolveAll(Type serviceType) => ResolveAllNamed(serviceType, null);

        public virtual IReadOnlyList<object> ResolveAllNamed(Type serviceType, string serviceName) => ServiceProvider.GetServices(serviceType)?.ToList();

        public virtual IReadOnlyList<TService> ResolveAll<TService>() where TService : class => ResolveAllNamed<TService>(null);

        public virtual IReadOnlyList<TService> ResolveAllNamed<TService>(string serviceName) where TService : class => ServiceProvider.GetServices<TService>()?.ToList();

        public virtual bool TryResolve(Type serviceType, out object service) => TryResolveNamed(serviceType, null, out service);

        public virtual bool TryResolveNamed(Type serviceType, string serviceName, out object service)
        {
            service = ResolveNamed(serviceType, serviceName);

            if (service != null)
                return true;

            return false;
        }

        public virtual bool TryResolve<TService>(out TService service) where TService : class => TryResolveNamed(null, out service);

        public virtual bool TryResolveNamed<TService>(string serviceName, out TService service)
            where TService : class
        {
            service = ResolveNamed<TService>(serviceName);

            if (service != null)
                return true;

            return false;
        }

        public virtual bool TryResolveAll(Type serviceType, out IReadOnlyList<object> services) => TryResolveAllNamed(serviceType, null, out services);

        public virtual bool TryResolveAllNamed(Type serviceType, string serviceName, out IReadOnlyList<object> services)
        {
            services = ResolveAllNamed(serviceType, serviceName);

            if (services?.Count > 0)
                return true;

            services = null;
            return false;
        }

        public virtual bool TryResolveAll<TService>(out IReadOnlyList<TService> services) where TService : class => TryResolveAllNamed(null, out services);

        public virtual bool TryResolveAllNamed<TService>(string serviceName, out IReadOnlyList<TService> services)
            where TService : class
        {
            services = ResolveAllNamed<TService>(serviceName);

            if (services?.Count > 0)
                return true;

            services = null;
            return false;
        }

        public virtual IScopedResolver CreateScope() => new ScopedResolver(ServiceProvider.CreateScope());

        #endregion
    }
}
