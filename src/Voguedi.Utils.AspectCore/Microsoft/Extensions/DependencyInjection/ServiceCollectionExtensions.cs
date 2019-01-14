using System;
using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using AspectCore.DynamicProxy.Parameters;
using AspectCore.Extensions.AspectScope;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddAspectCore(this IServiceCollection services, Action<IAspectConfiguration> aspectConfig = null)
        {
            services.ConfigureDynamicProxy(c =>
            {
                c.EnableParameterAspect();
                aspectConfig?.Invoke(c);
            });
            services.TryAddScoped<IAspectScheduler, ScopeAspectScheduler>();
            services.TryAddScoped<IAspectBuilderFactory, ScopeAspectBuilderFactory>();
            services.TryAddScoped<IAspectContextFactory, ScopeAspectContextFactory>();
            return services;
        }

        #endregion
    }
}
