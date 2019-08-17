using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using Voguedi.RabbitMQ;

namespace Voguedi.Messages.RabbitMQ
{
    class RabbitMQMessageProducer : IMessageProducer
    {
        #region Private Fields

        readonly IRabbitMQChannelPool channelPool;
        readonly string exchangeName;

        #endregion

        #region Ctors

        public RabbitMQMessageProducer(IRabbitMQChannelPool channelPool, IOptions<RabbitMQOptions> options)
        {
            this.channelPool = channelPool;
            exchangeName = options.Value.ExchangeName;
        }

        #endregion

        #region IMessageProducer

        public Task<OperatedResult> ProduceAsync(string topic, string content)
        {
            var channel = default(IModel);

            try
            {
                channel = channelPool.Get();
                channel.ExchangeDeclare(exchangeName, RabbitMQOptions.ExchangeType, true);
                channel.BasicPublish(exchangeName, topic, new BasicProperties { DeliveryMode = 2 }, Encoding.UTF8.GetBytes(content));
                return Task.FromResult(OperatedResult.Success);
            }
            catch (Exception ex)
            {
                return Task.FromResult(OperatedResult.Failed(ex));
            }
            finally
            {
                if (channel != null && !channelPool.TryReturn(channel))
                    channel.Dispose();
            }
        }

        #endregion
    }
}
