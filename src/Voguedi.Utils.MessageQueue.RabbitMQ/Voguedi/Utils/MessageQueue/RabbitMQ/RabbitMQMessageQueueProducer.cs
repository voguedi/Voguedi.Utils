using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Voguedi.Utils.AsyncExecutors;
using Voguedi.Utils.RabbitMQ;

namespace Voguedi.Utils.MessageQueue.RabbitMQ
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

        public Task<AsyncExecutionResult> ProduceAsync(string queueTopic, string queueMessage)
        {
            var channel = channelPool.Pull();

            try
            {
                channel.ExchangeDeclare(exchangeName, exchangeType, true);
                channel.BasicPublish(exchangeName, queueTopic, null, Encoding.UTF8.GetBytes(queueMessage));
                logger.LogInformation($"消息生产成功！ [ExchangeName = {exchangeName}, ExchangeType = {exchangeType}, QueueTopic = {queueTopic}, QueueMessage = {queueMessage}]");
                return Task.FromResult(AsyncExecutionResult.Success);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"消息生产失败！ [ExchangeName = {exchangeName}, ExchangeType = {exchangeType}, QueueTopic = {queueTopic}, QueueMessage = {queueMessage}]");
                return Task.FromResult(AsyncExecutionResult.Failed(ex));
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
