using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.ObjectSerialization;
using Voguedi.ObjectSerialization.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddJson(this IServiceCollection services)
        {
            services.TryAddTransient<IStringObjectSerializer, JsonObjectSerializer>();
            return services;
        }

        #endregion
    }
}
