using Microsoft.Extensions.Options;

namespace Voguedi.Messages.Kafka
{
    class KafkaMessageConsumerFactory : IMessageConsumerFactory
    {
        #region Private Fields

        readonly KafkaOptions options;

        #endregion

        #region Ctors

        public KafkaMessageConsumerFactory(IOptions<KafkaOptions> options) => this.options = options.Value;

        #endregion

        #region IMessageConsumerFactory

        public IMessageConsumer Create(string group) => new KafkaMessageConsumer(group, options);

        #endregion
    }
}
