using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi;
using Voguedi.Kafka;
using Voguedi.Messaging;
using Voguedi.Messaging.Kafka;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddKafka(this IServiceCollection services, Action<KafkaOptions> setupAction)
        {
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            var options = new KafkaOptions();
            setupAction.Invoke(options);
            services.AddSingleton(options);
            services.TryAddSingleton<IKafkaProducerPool, KafkaProducerPool>();
            services.TryAddSingleton<IMessageConsumerFactory, KafkaMessageConsumerFactory>();
            services.TryAddSingleton<IMessageProducer, KafkaMessageProducer>();
            return services;
        }

        public static IServiceCollection AddKafka(this IServiceCollection services, string servers)
        {
            if (string.IsNullOrWhiteSpace(servers))
                throw new ArgumentNullException(nameof(servers));

            return services.AddKafka(s => s.Servers = servers);
        }

        #endregion
    }
}
