using System;
using System.Collections.Generic;
using System.Linq;
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
            var serviceTypes = default(IEnumerable<Type>);

            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<IScopedDependency>(assemblies))
            {
                serviceTypes = implementationType.GetTypeInfo().ImplementedInterfaces.Where(i => i != typeof(IScopedDependency));

                if (serviceTypes?.Count() > 0)
                {
                    foreach (var serviceType in serviceTypes)
                        services.TryAddEnumerable(ServiceDescriptor.Scoped(serviceType, implementationType));
                }
                else
                    services.TryAddScoped(implementationType, implementationType);
            }

            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<ISingletonDependency>(assemblies))
            {
                serviceTypes = implementationType.GetTypeInfo().ImplementedInterfaces.Where(i => i != typeof(ISingletonDependency));

                if (serviceTypes?.Count() > 0)
                {
                    foreach (var serviceType in serviceTypes)
                        services.TryAddEnumerable(ServiceDescriptor.Singleton(serviceType, implementationType));
                }
                else
                    services.TryAddSingleton(implementationType, implementationType);
            }

            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<ITransientDependency>(assemblies))
            {
                serviceTypes = implementationType.GetTypeInfo().ImplementedInterfaces.Where(i => i != typeof(ITransientDependency));

                if (serviceTypes?.Count() > 0)
                {
                    foreach (var serviceType in serviceTypes)
                        services.TryAddEnumerable(ServiceDescriptor.Transient(serviceType, implementationType));
                }
                else
                    services.TryAddScoped(implementationType, implementationType);
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
