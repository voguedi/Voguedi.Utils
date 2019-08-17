using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi;
using Voguedi.Messages;
using Voguedi.Messages.RabbitMQ;
using Voguedi.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RabbitMQServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, Action<RabbitMQOptions> setupAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            services.Configure(setupAction);
            services.TryAddSingleton<IRabbitMQConnectionProvider, RabbitMQConnectionProvider>();
            services.TryAddSingleton<IRabbitMQChannelPool, RabbitMQChannelPool>();
            services.TryAddSingleton<IMessageConsumerFactory, RabbitMQMessageConsumerFactory>();
            services.TryAddSingleton<IMessageProducer, RabbitMQMessageProducer>();
            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services) => services.AddRabbitMQ(_ => { });

        #endregion
    }
}
