using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.Utils;
using Voguedi.Utils.MessageQueue;
using Voguedi.Utils.MessageQueue.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        #region Public Methods

        public static IServiceCollection AddRabbitMQMessageQueue(this IServiceCollection services, Action<RabbitMQOptions> setupAction)
        {
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));

            services.AddRabbitMQ(setupAction);
            services.TryAddSingleton<IMessageQueueConsumerFactory, RabbitMQMessageQueueConsumerFactory>();
            services.TryAddSingleton<IMessageQueueProducer, RabbitMQMessageQueueProducer>();
            return services;
        }

        public static IServiceCollection AddRabbitMQMessageQueue(this IServiceCollection services, string hostName = "localhost")
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentNullException(nameof(hostName));
            
            return services.AddRabbitMQMessageQueue(s => s.HostName = hostName);
        }

        #endregion
    }
}
