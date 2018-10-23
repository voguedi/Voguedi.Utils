using Microsoft.Extensions.Logging;

namespace Voguedi.Utils.MessageQueue.Kafka
{
    class KafkaMessageQueueConsumerFactory : IMessageQueueConsumerFactory
    {
        #region Private Fields

        readonly ILoggerFactory loggerFactory;
        readonly KafkaOptions options;

        #endregion

        #region Ctors

        public KafkaMessageQueueConsumerFactory(ILoggerFactory loggerFactory, KafkaOptions options)
        {
            this.loggerFactory = loggerFactory;
            this.options = options;
        }

        #endregion

        #region IMessageQueueConsumerFactory

        public IMessageQueueConsumer Create(string queueName) => new KafkaMessageQueueConsumer(queueName, loggerFactory.CreateLogger<KafkaMessageQueueConsumer>(), options);

        #endregion
    }
}
