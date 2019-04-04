using Voguedi.RabbitMQ;

namespace Voguedi.MessageQueues.RabbitMQ
{
    class RabbitMQMessageQueueConsumerFactory : IMessageQueueConsumerFactory
    {
        #region Private Fields

        readonly IRabbitMQConnectionPool connectionPool;
        readonly RabbitMQOptions options;

        #endregion

        #region Ctors

        public RabbitMQMessageQueueConsumerFactory(IRabbitMQConnectionPool connectionPool, RabbitMQOptions options)
        {
            this.connectionPool = connectionPool;
            this.options = options;
        }

        #endregion

        #region IMessageQueueConsumerFactory

        public IMessageQueueConsumer Create(string queueName) => new RabbitMQMessageQueueConsumer(queueName, connectionPool, options);

        #endregion
    }
}
