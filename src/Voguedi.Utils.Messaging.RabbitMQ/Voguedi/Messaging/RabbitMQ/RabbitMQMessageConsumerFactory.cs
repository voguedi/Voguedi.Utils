using Microsoft.Extensions.Logging;
using Voguedi.RabbitMQ;

namespace Voguedi.Messaging.RabbitMQ
{
    class RabbitMQMessageConsumerFactory : IMessageConsumerFactory
    {
        #region Private Fields

        readonly IRabbitMQConnectionPool connectionPool;
        readonly ILoggerFactory loggerFactory;
        readonly RabbitMQOptions options;

        #endregion

        #region Ctors

        public RabbitMQMessageConsumerFactory(IRabbitMQConnectionPool connectionPool, ILoggerFactory loggerFactory, RabbitMQOptions options)
        {
            this.connectionPool = connectionPool;
            this.loggerFactory = loggerFactory;
            this.options = options;
        }

        #endregion

        #region IMessageConsumerFactory

        public IMessageConsumer Create(string queueName) => new RabbitMQMessageConsumer(queueName, connectionPool, loggerFactory.CreateLogger<RabbitMQMessageConsumer>(), options);

        #endregion
    }
}
