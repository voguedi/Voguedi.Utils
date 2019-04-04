using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using Voguedi.Infrastructure;
using Voguedi.RabbitMQ;

namespace Voguedi.MessageQueues.RabbitMQ
{
    class RabbitMQMessageQueueProducer : IMessageQueueProducer
    {
        #region Private Fields

        readonly IRabbitMQChannelPool channelPool;
        readonly ILogger logger;
        readonly string exchangeName;
        readonly string exchangeType;

        #endregion

        #region Ctors

        public RabbitMQMessageQueueProducer(IRabbitMQChannelPool channelPool, ILogger<RabbitMQMessageQueueProducer> logger, RabbitMQOptions options)
        {
            this.channelPool = channelPool;
            this.logger = logger;
            exchangeName = options.ExchangeName;
            exchangeType = options.ExchangeType;
        }

        #endregion

        #region IMessageQueueProducer

        public Task<AsyncExecutedResult> ProduceAsync(string queueTopic, string queueMessage)
        {
            var channel = channelPool.Pull();

            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, true);
                channel.BasicPublish(exchangeName, queueTopic, new BasicProperties { DeliveryMode = 2 }, Encoding.UTF8.GetBytes(queueMessage));
                return Task.FromResult(AsyncExecutedResult.Success);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"生产消息失败！ [QueueTopic = {queueTopic}, QueueMessage = {queueMessage}]");
                return Task.FromResult(AsyncExecutedResult.Failed(ex));
            }
            finally
            {
                if (!channelPool.Push(channel))
                    channel.Dispose();
            }
        }

        #endregion
    }
}
