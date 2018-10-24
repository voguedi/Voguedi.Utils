using Microsoft.Extensions.Logging;

namespace Voguedi.Messaging.Kafka
{
    class KafkaMessageConsumerFactory : IMessageConsumerFactory
    {
        #region Private Fields

        readonly ILoggerFactory loggerFactory;
        readonly KafkaOptions options;

        #endregion

        #region Ctors

        public KafkaMessageConsumerFactory(ILoggerFactory loggerFactory, KafkaOptions options)
        {
            this.loggerFactory = loggerFactory;
            this.options = options;
        }

        #endregion

        #region IMessageConsumerFactory

        public IMessageConsumer Create(string queueName) => new KafkaMessageConsumer(queueName, loggerFactory.CreateLogger<KafkaMessageConsumer>(), options);

        #endregion
    }
}
