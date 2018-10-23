using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.Utils;
using Voguedi.Utils.MessageQueue;
using Voguedi.Utils.MessageQueue.Kafka;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddKafkaMessageQueue(this IServiceCollection services, Action<KafkaOptions> setupAction)
        {
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            services.AddKafka(setupAction);
            services.TryAddSingleton<IMessageQueueConsumerFactory, KafkaMessageQueueConsumerFactory>();
            services.TryAddSingleton<IMessageQueueProducer, KafkaMessageQueueProducer>();
            return services;
        }

        public static IServiceCollection AddRabbitMQMessageQueue(this IServiceCollection services, string servers)
        {
            if (string.IsNullOrWhiteSpace(servers))
                throw new ArgumentNullException(nameof(servers));
            
            return services.AddKafkaMessageQueue(s => s.Servers = servers);
        }

        #endregion
    }
}
