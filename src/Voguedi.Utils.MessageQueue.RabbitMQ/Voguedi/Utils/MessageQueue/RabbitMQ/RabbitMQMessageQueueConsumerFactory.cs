using Microsoft.Extensions.Logging;
using Voguedi.Utils.RabbitMQ;

namespace Voguedi.Utils.MessageQueue.RabbitMQ
{
    class RabbitMQMessageQueueConsumerFactory : IMessageQueueConsumerFactory
    {
        #region Private Fields

        readonly IRabbitMQConnectionPool connectionPool;
        readonly ILoggerFactory loggerFactory;
        readonly RabbitMQOptions options;

        #endregion

        #region Ctors

        public RabbitMQMessageQueueConsumerFactory(IRabbitMQConnectionPool connectionPool, ILoggerFactory loggerFactory, RabbitMQOptions options)
        {
            this.connectionPool = connectionPool;
            this.loggerFactory = loggerFactory;
            this.options = options;
        }

        #endregion

        #region IMessageQueueConsumerFactory

        public IMessageQueueConsumer Create(string queueName) => new RabbitMQMessageQueueConsumer(queueName, connectionPool, loggerFactory.CreateLogger<RabbitMQMessageQueueConsumer>(), options);

        #endregion
    }
}
