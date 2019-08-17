using Microsoft.Extensions.Options;
using Voguedi.RabbitMQ;

namespace Voguedi.Messages.RabbitMQ
{
    public class RabbitMQMessageConsumerFactory : IMessageConsumerFactory
    {
        #region Private Fields

        readonly IRabbitMQConnectionProvider connectionProvider;
        readonly RabbitMQOptions options;

        #endregion

        #region Ctors

        public RabbitMQMessageConsumerFactory(IRabbitMQConnectionProvider connectionProvider, IOptions<RabbitMQOptions> options)
        {
            this.connectionProvider = connectionProvider;
            this.options = options.Value;
        }

        #endregion

        #region IMessageConsumerFactory

        public IMessageConsumer Create(string group) => new RabbitMQMessageConsumer(group, connectionProvider, options);

        #endregion
    }
}
