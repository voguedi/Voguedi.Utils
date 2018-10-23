using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Voguedi.Utils.AsyncExecutors;
using Voguedi.Utils.Kafka;

namespace Voguedi.Utils.MessageQueue.Kafka
{
    class KafkaMessageQueueProducer : IMessageQueueProducer
    {
        #region Private Fields

        readonly IKafkaProducerPool producerPool;
        readonly ILogger logger;

        #endregion

        #region Ctors

        public KafkaMessageQueueProducer(IKafkaProducerPool producerPool, ILogger<KafkaMessageQueueProducer> logger)
        {
            this.producerPool = producerPool;
            this.logger = logger;
        }

        #endregion

        #region IMessageQueueProducer

        public async Task<AsyncExecutionResult> ProduceAsync(string queueTopic, string queueMessage)
        {
            var producer = producerPool.Pull();

            try
            {
                var message = await producer.ProduceAsync(queueTopic, null, Encoding.UTF8.GetBytes(queueMessage));

                if (!message.Error.HasError)
                {
                    logger.LogInformation($"消息生产成功！ [QueueTopic = {queueTopic}, QueueMessage = {queueMessage}]");
                    return AsyncExecutionResult.Success;
                }

                throw new Exception(message.Error.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"消息生产失败！ [QueueTopic = {queueTopic}, QueueMessage = {queueMessage}]");
                return AsyncExecutionResult.Failed(ex);
            }
            finally
            {
                if (!producerPool.Push(producer))
                    producer.Dispose();
            }
        }

        #endregion
    }
}
