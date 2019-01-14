using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Voguedi.DisposableObjects;

namespace Voguedi.DependencyInjection
{
    public class ScopedResolver : DisposableObject, IScopedResolver
    {
        #region Private Fields

        readonly IServiceScope serviceScope;
        bool disposed;

        #endregion

        #region Ctors

        public ScopedResolver(IServiceScope serviceScope) => this.serviceScope = serviceScope;

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    serviceScope.Dispose();

                disposed = true;
            }
        }

        #endregion

        #region IScopedResolver

        public virtual object Resolve(Type serviceType) => ResolveNamed(serviceType, null);

        public virtual object ResolveNamed(Type serviceType, string serviceName) => serviceScope.ServiceProvider.GetService(serviceType);

        public virtual TService Resolve<TService>() where TService : class => ResolveNamed<TService>(null);

        public virtual TService ResolveNamed<TService>(string serviceName) where TService : class => serviceScope.ServiceProvider.GetService<TService>();

        public virtual IReadOnlyList<object> ResolveAll(Type serviceType) => ResolveAllNamed(serviceType, null);

        public virtual IReadOnlyList<object> ResolveAllNamed(Type serviceType, string serviceName) => serviceScope.ServiceProvider.GetServices(serviceType)?.ToList();

        public virtual IReadOnlyList<TService> ResolveAll<TService>() where TService : class => ResolveAllNamed<TService>(null);

        public virtual IReadOnlyList<TService> ResolveAllNamed<TService>(string serviceName) where TService : class => serviceScope.ServiceProvider.GetServices<TService>()?.ToList();

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
            service = ResolveNamed<TService>(null);

            if (service != null)
                return true;

            return false;
        }

        public virtual bool TryResolveAll(Type serviceType, out IReadOnlyList<object> services) => TryResolveAllNamed(serviceType, null, out services);

        public virtual bool TryResolveAllNamed(Type serviceType, string serviceName, out IReadOnlyList<object> services)
        {
            services = ResolveAllNamed(serviceType, null);

            if (services?.Count > 0)
                return true;

            services = null;
            return false;
        }

        public virtual bool TryResolveAll<TService>(out IReadOnlyList<TService> services) where TService : class => TryResolveAllNamed(null, out services);

        public virtual bool TryResolveAllNamed<TService>(string serviceName, out IReadOnlyList<TService> services)
            where TService : class
        {
            services = ResolveAllNamed<TService>(null);

            if (services?.Count > 0)
                return true;

            services = null;
            return false;
        }

        #endregion
    }
}
