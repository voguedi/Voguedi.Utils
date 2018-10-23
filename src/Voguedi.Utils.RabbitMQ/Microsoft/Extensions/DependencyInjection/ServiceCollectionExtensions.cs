﻿using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voguedi.Utils;
using Voguedi.Utils.RabbitMQ;

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
            return services;
        }

        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, string hostName = "localhost")
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentNullException(nameof(hostName));

            return services.AddRabbitMQ(s => s.HostName = hostName);
        }

        #endregion
    }
}