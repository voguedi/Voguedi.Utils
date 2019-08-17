using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi;
using Voguedi.Kafka;
using Voguedi.Messages;
using Voguedi.Messages.Kafka;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KafkaServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddKafka(this IServiceCollection services, Action<KafkaOptions> setupAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.Configure(setupAction);
            services.TryAddSingleton<IKafkaProducerPool, KafkaProducerPool>();
            services.TryAddSingleton<IMessageConsumerFactory, KafkaMessageConsumerFactory>();
            services.TryAddSingleton<IMessageProducer, KafkaMessageProducer>();
            return services;
        }

        #endregion
    }
}
