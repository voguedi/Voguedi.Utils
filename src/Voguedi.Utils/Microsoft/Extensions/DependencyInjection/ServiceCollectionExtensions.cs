using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.AsyncExecution;
using Voguedi.BackgroundWorkers;
using Voguedi.DependencyInjection;
using Voguedi.ObjectSerialization;
using Voguedi.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Internal Methods

        internal static IServiceCollection AddDependencies(this IServiceCollection services, params Assembly[] assemblies)
        {
            var typeFinder = new TypeFinder();

            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<IScopedDependency>(assemblies))
            {
                foreach (var serviceType in implementationType.GetTypeInfo().ImplementedInterfaces)
                    services.TryAddEnumerable(ServiceDescriptor.Scoped(serviceType, implementationType));
            }

            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<ISingletonDependency>(assemblies))
            {
                foreach (var serviceType in implementationType.GetTypeInfo().ImplementedInterfaces)
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(serviceType, implementationType));
            }

            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<ITransientDependency>(assemblies))
            {
                foreach (var serviceType in implementationType.GetTypeInfo().ImplementedInterfaces)
                    services.TryAddEnumerable(ServiceDescriptor.Transient(serviceType, implementationType));
            }

            return services;
        }

        #endregion

        #region Public Methods

        public static IServiceCollection AddUitls(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.TryAddSingleton<IAsyncExecutor, AsyncExecutor>();
            services.TryAddSingleton<IBackgroundWorker, BackgroundWorker>();
            services.TryAddSingleton<IBinaryObjectSerializer, BinaryObjectSerializer>();
            services.TryAddSingleton<ITypeFinder, TypeFinder>();
            services.AddDependencies(assemblies);
            return services;
        }

        #endregion
    }
}
