using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Voguedi.RabbitMQ
{
    class RabbitMQConnectionProvider : DisposableObject, IRabbitMQConnectionProvider
    {
        #region Private Fields

        readonly ILogger logger;
        readonly Func<IConnection> connectionActivator;
        IConnection connection;
        bool disposed;

        #endregion

        #region Ctors

        public RabbitMQConnectionProvider(ILogger<RabbitMQConnectionProvider> logger, RabbitMQOptions options)
        {
            this.logger = logger;
            connectionActivator = BuildConnectionActivator(options);
        }

        #endregion

        #region Private Methods

        static Func<IConnection> BuildConnectionActivator(RabbitMQOptions options)
        {
            var factory = new ConnectionFactory
            {
                Password = options.Password,
                Port = options.Port,
                UserName = options.UserName,
                VirtualHost = options.VirtualHost
            };

            if (options.HostName.Contains(","))
            {
                var hostNames = options.HostName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                return () => factory.CreateConnection(hostNames);
            }

            factory.HostName = options.HostName;
            return () => factory.CreateConnection();
        }

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                connection?.Dispose();
                disposed = true;
            }
        }

        #endregion

        #region IRabbitMQConnectionProvider

        public IConnection Get()
        {
            if (connection != null && connection.IsOpen)
                return connection;

            connection = connectionActivator();
            connection.ConnectionShutdown += (sender, e) => logger.LogError($"RabbitMQ connection shutdown! [Reason = {e.ReplyText}]");
            return connection;
        }

        #endregion
    }
}
