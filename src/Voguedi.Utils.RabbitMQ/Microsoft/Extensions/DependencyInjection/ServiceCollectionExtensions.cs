using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi;
using Voguedi.MessageQueues;
using Voguedi.MessageQueues.RabbitMQ;
using Voguedi.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> setupAction)
        {
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            var options = new RabbitMQOptions();
            setupAction.Invoke(options);
            services.AddSingleton(options);
            services.TryAddSingleton<IRabbitMQConnectionPool, RabbitMQConnectionPool>();
            services.TryAddSingleton<IRabbitMQChannelPool, RabbitMQChannelPool>();
            services.TryAddSingleton<IMessageQueueConsumerFactory, RabbitMQMessageQueueConsumerFactory>();
            services.TryAddSingleton<IMessageQueueProducer, RabbitMQMessageQueueProducer>();
            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, string hostName, string exchangeName)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentNullException(nameof(hostName));

            if (string.IsNullOrWhiteSpace(exchangeName))
                throw new ArgumentNullException(nameof(exchangeName));

            return services.AddRabbitMQ(s =>
            {
                s.HostName = hostName;
                s.ExchangeName = exchangeName;
            });
        }

        #endregion
    }
}
