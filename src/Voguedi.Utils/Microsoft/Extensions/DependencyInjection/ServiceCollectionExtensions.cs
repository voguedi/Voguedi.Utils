using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.Utils.DependencyInjection;
using Voguedi.Utils.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Private Methods

        static IReadOnlyList<Type> GetServiceTypes(Type implementationType)
        {
            var serviceTypes = new List<Type>();

            foreach (var i in implementationType.GetTypeInfo().GetInterfaces())
            {
                if (implementationType.Name.EndsWith(i.Name.Substring(1, i.Name.Length - 1)))
                    serviceTypes.Add(i);
            }

            return serviceTypes;
        }

        static void AddTypes(IServiceCollection services, Type specifiedType, ServiceLifetime lifetime)
        {
            var implementationTypes = TypeFinder.Instance.GetTypes().Where(t => t.IsClass && !t.IsAbstract && specifiedType.IsAssignableFrom(t));

            if (implementationTypes?.Count() > 0)
            {
                foreach (var implementationType in implementationTypes)
                {
                    foreach (var serviceType in GetServiceTypes(implementationType))
                        services.TryAddEnumerable(new ServiceDescriptor(serviceType, implementationType, lifetime));
                }
            }
        }

        static void AddScopedTypes(IServiceCollection services) => AddTypes(services, typeof(IScopedDependency), ServiceLifetime.Scoped);

        static void AddSingletonTypes(IServiceCollection services) => AddTypes(services, typeof(ISingletonDependency), ServiceLifetime.Singleton);

        static void AddTransientTypes(IServiceCollection services) => AddTypes(services, typeof(ISingletonDependency), ServiceLifetime.Transient);

        #endregion

        #region Public Methods

        public static IServiceCollection AddDependencyTypes(this IServiceCollection services)
        {
            AddScopedTypes(services);
            AddSingletonTypes(services);
            AddTransientTypes(services);
            return services;
        }

        #endregion
    }
}
