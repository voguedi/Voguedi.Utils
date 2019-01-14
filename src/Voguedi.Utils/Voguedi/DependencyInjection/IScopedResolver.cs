using System;
using System.Collections.Generic;

namespace Voguedi.DependencyInjection
{
    public interface IScopedResolver : IDisposable
    {
        #region Methods

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

        #endregion
    }
}
