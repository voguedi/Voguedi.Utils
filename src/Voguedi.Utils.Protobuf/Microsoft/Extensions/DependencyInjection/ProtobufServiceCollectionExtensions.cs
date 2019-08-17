using System;
using Voguedi.ObjectSerializers;
using Voguedi.ObjectSerializers.Protobuf;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ProtobufServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddProtobuf(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IBinarySerializer, ProtobufBinarySerializer>();
            return services;
        }

        #endregion
    }
}
