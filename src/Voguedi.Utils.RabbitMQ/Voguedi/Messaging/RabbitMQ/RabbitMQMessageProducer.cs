using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Voguedi.AsyncExecution;
using Voguedi.RabbitMQ;

namespace Voguedi.Messaging.RabbitMQ
{
    class RabbitMQMessageProducer : IMessageProducer
    {
        #region Private Fields

        readonly IRabbitMQChannelPool channelPool;
        readonly ILogger logger;
        readonly string exchangeName;
        readonly string exchangeType;

        #endregion

        #region Ctors

        public RabbitMQMessageProducer(IRabbitMQChannelPool channelPool, ILogger<RabbitMQMessageProducer> logger, RabbitMQOptions options)
        {
            this.channelPool = channelPool;
            this.logger = logger;
            exchangeName = options.ExchangeName;
            exchangeType = options.ExchangeType;
        }

        #endregion

        #region IMessageProducer

        public Task<AsyncExecutedResult> ProduceAsync(string queueTopic, string queueMessage)
        {
            var channel = channelPool.Pull();

            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, true);
                channel.BasicPublish(exchangeName, queueTopic, null, Encoding.UTF8.GetBytes(queueMessage));
                return Task.FromResult(AsyncExecutedResult.Success);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"消息生产失败！ [QueueTopic = {queueTopic}, QueueMessage = {queueMessage}]");
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
