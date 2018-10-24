using System;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Voguedi.RabbitMQ
{
    class RabbitMQConnectionPool : IRabbitMQConnectionPool
    {
        #region Private Fields

        readonly ILogger logger;
        readonly Func<IConnection> connectionFactory;
        IConnection connection;

        #endregion

        #region Ctors

        public RabbitMQConnectionPool(ILogger<RabbitMQConnectionPool> logger, RabbitMQOptions options)
        {
            this.logger = logger;
            connectionFactory = BuildConnectionFactory(options);
        }

        #endregion

        #region Private Methods

        static Func<IConnection> BuildConnectionFactory(RabbitMQOptions options)
        {
            var factory = new ConnectionFactory
            {
                UserName = options.UserName,
                Port = options.Port,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                SocketReadTimeout = options.SocketReadTimeout,
                SocketWriteTimeout = options.SocketWriteTimeout
            };
            var hostName = options.HostName;

            if (hostName.Contains(","))
                return () => factory.CreateConnection(hostName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));

            factory.HostName = hostName;
            return () => factory.CreateConnection();
        }

        #endregion

        #region IRabbitMQConnectionPool

        public IConnection Pull()
        {
            if (connection != null && connection.IsOpen)
                return connection;

            connection = connectionFactory();
            connection.ConnectionShutdown += (sender, e) => logger.LogError($"连接已关闭！原因：{e.ReplyText}");
            return connection;
        }

        #endregion
    }
}
