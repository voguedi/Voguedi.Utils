using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.AsyncExecution;
using Voguedi.BackgroundWorkers;
using Voguedi.DependencyInjection;
using Voguedi.ObjectSerializing;
using Voguedi.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Private Fields

        static void AddScopedServices(IServiceCollection services, TypeFinder typeFinder, params Assembly[] assemblies)
        {
            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<IScopedDependency>(assemblies))
            {
                foreach (var serviceType in implementationType.GetTypeInfo().ImplementedInterfaces)
                    services.TryAddEnumerable(ServiceDescriptor.Scoped(serviceType, implementationType));
            }
        }

        static void AddSingletonServices(IServiceCollection services, TypeFinder typeFinder, params Assembly[] assemblies)
        {
            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<ISingletonDependency>(assemblies))
            {
                foreach (var serviceType in implementationType.GetTypeInfo().ImplementedInterfaces)
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(serviceType, implementationType));
            }
        }

        static void AddTransientServices(IServiceCollection services, TypeFinder typeFinder, params Assembly[] assemblies)
        {
            foreach (var implementationType in typeFinder.GetTypesBySpecifiedType<ITransientDependency>(assemblies))
            {
                foreach (var serviceType in implementationType.GetTypeInfo().ImplementedInterfaces)
                    services.TryAddEnumerable(ServiceDescriptor.Transient(serviceType, implementationType));
            }
        }

        #endregion

        #region Public Methods

        public static IServiceCollection AddUitls(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IAsyncExecutor, AsyncExecutor>();
            services.TryAddSingleton<IBackgroundWorker, BackgroundWorker>();
            services.TryAddSingleton<IBinaryObjectSerializer, BinaryObjectSerializer>();
            services.TryAddSingleton<ITypeFinder, TypeFinder>();
            return services;
        }

        public static IServiceCollection AddDependencyServices(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            
            var typeFinder = new TypeFinder();
            AddScopedServices(services, typeFinder, assemblies);
            AddSingletonServices(services, typeFinder, assemblies);
            AddTransientServices(services, typeFinder, assemblies);
            return services;
        }

        #endregion
    }
}
