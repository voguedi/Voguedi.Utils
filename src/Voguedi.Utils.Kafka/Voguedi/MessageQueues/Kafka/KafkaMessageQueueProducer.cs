using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Voguedi.Infrastructure;
using Voguedi.Kafka;

namespace Voguedi.MessageQueues.Kafka
{
    class KafkaMessageProducer : IMessageQueueProducer
    {
        #region Private Fields

        readonly IKafkaProducerPool producerPool;
        readonly ILogger logger;

        #endregion

        #region Ctors

        public KafkaMessageProducer(IKafkaProducerPool producerPool, ILogger<KafkaMessageProducer> logger)
        {
            this.producerPool = producerPool;
            this.logger = logger;
        }

        #endregion

        #region IMessageQueueProducer

        public async Task<AsyncExecutedResult> ProduceAsync(string queueTopic, string queueMessage)
        {
            var producer = producerPool.Pull();

            try
            {
                var message = await producer.ProduceAsync(queueTopic, null, Encoding.UTF8.GetBytes(queueMessage));

                if (!message.Error.HasError)
                    return AsyncExecutedResult.Success;

                throw new Exception(message.Error.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"生产消息失败！ [QueueTopic = {queueTopic}, QueueMessage = {queueMessage}]");
                return AsyncExecutedResult.Failed(ex);
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
