using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.ObjectSerializers;
using Voguedi.ObjectSerializers.JsonNet;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class JsonNetServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddJsonNet(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IStringSerializer, JsonNetStringSerializer>();
            return services;
        }

        #endregion
    }
}
