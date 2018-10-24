﻿using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi;
using Voguedi.Messaging;
using Voguedi.Messaging.Kafka;

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
            services.TryAddSingleton<IMessageConsumerFactory, KafkaMessageConsumerFactory>();
            services.TryAddSingleton<IMessageProducer, KafkaMessageProducer>();
            return services;
        }

        public static IServiceCollection AddRabbitMQMessageQueue(this IServiceCollection services, string servers) => services.AddKafkaMessageQueue(s => s.Servers = servers);

        #endregion
    }
}
