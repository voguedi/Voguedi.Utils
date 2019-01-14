using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Voguedi.DisposableObjects;

namespace Voguedi.DependencyInjection.Autofac
{
    public class AutofacScopeResolver : DisposableObject, IScopedResolver
    {
        #region Private Fields

        readonly ILifetimeScope lifetimeScope;
        bool disposed;

        #endregion

        #region Ctors

        public AutofacScopeResolver(ILifetimeScope lifetimeScope) => this.lifetimeScope = lifetimeScope;

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    lifetimeScope.Dispose();

                disposed = true;
            }
        }

        #endregion

        #region IAutofacScopeResolver

        public object Resolve(Type serviceType) => ResolveNamed(serviceType, null);

        public object ResolveNamed(Type serviceType, string serviceName)
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
                return lifetimeScope.ResolveNamed(serviceName, serviceType);

            return lifetimeScope.Resolve(serviceType);
        }

        public TService Resolve<TService>() where TService : class => ResolveNamed<TService>(null);

        public TService ResolveNamed<TService>(string serviceName)
            where TService : class
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
                return lifetimeScope.ResolveNamed<TService>(serviceName);

            return lifetimeScope.Resolve<TService>();
        }

        public IReadOnlyList<object> ResolveAll(Type serviceType) => ResolveAllNamed(serviceType, null);

        public IReadOnlyList<TService> ResolveAll<TService>() where TService : class => ResolveAllNamed<TService>(null);

        public IReadOnlyList<object> ResolveAllNamed(Type serviceType, string serviceName)
        {
            var implementations = default(IEnumerable<object>);

            if (!string.IsNullOrWhiteSpace(serviceName))
                implementations = (IEnumerable<object>)ResolveNamed(typeof(IEnumerable<>).MakeGenericType(serviceType), serviceName);
            else
                implementations = (IEnumerable<object>)Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType));

            return implementations?.ToList();
        }

        public IReadOnlyList<TService> ResolveAllNamed<TService>(string serviceName) where TService : class => ResolveAllNamed(typeof(TService), serviceName)?.Cast<TService>()?.ToList();

        public bool TryResolve(Type serviceType, out object service) => TryResolveNamed(serviceType, null, out service);

        public bool TryResolveNamed(Type serviceType, string serviceName, out object service)
        {
            if (!string.IsNullOrWhiteSpace(serviceName))
                return lifetimeScope.TryResolveNamed(serviceName, serviceType, out service);

            return lifetimeScope.TryResolve(serviceType, out service);
        }

        public bool TryResolve<TService>(out TService service) where TService : class => TryResolveNamed(null, out service);

        public bool TryResolveNamed<TService>(string serviceName, out TService service) where TService : class
        {
            service = ResolveNamed<TService>(serviceName);

            if (service != null)
                return true;

            return false;
        }

        public bool TryResolveAll(Type serviceType, out IReadOnlyList<object> services) => TryResolveAllNamed(serviceType, null, out services);

        public bool TryResolveAllNamed(Type serviceType, string serviceName, out IReadOnlyList<object> services)
        {
            services = ResolveAllNamed(serviceType, serviceName);

            if (services?.Count > 0)
                return true;

            services = null;
            return false;
        }

        public bool TryResolveAll<TService>(out IReadOnlyList<TService> services) where TService : class => TryResolveAllNamed(null, out services);

        public bool TryResolveAllNamed<TService>(string serviceName, out IReadOnlyList<TService> services) where TService : class
        {
            services = ResolveAllNamed<TService>(serviceName);

            if (services?.Count > 0)
                return true;

            services = null;
            return false;
        }

        #endregion
    }
}
