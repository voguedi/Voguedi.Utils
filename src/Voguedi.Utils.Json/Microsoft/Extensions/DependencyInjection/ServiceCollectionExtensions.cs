using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.ObjectSerializing;
using Voguedi.ObjectSerializing.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddJson(this IServiceCollection services)
        {
            services.TryAddTransient<IStringObjectSerializer, JsonStringObjectSerializer>();
            return services;
        }

        #endregion
    }
}
