using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi;
using Voguedi.Messaging;
using Voguedi.Messaging.RabbitMQ;
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
            services.TryAddSingleton<IMessageConsumerFactory, RabbitMQMessageConsumerFactory>();
            services.TryAddSingleton<IMessageProducer, RabbitMQMessageProducer>();
            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, string hostName = "localhost") => services.AddRabbitMQ(s => s.HostName = hostName);

        #endregion
    }
}
