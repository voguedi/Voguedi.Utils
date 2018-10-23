using System;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;

namespace Voguedi.Utils.MessageQueue.Kafka
{
    class KafkaMessageQueueConsumer : DisposableObject, IMessageQueueConsumer
    {
        #region Private Fields

        readonly string queueName;
        readonly KafkaOptions options;
        readonly ILogger logger;
        readonly StringDeserializer deserializer = new StringDeserializer(Encoding.UTF8);
        Consumer<Null, string> consumer;
        bool disposed;

        #endregion

        #region Ctors

        public KafkaMessageQueueConsumer(string queueName, ILogger<KafkaMessageQueueConsumer> logger, KafkaOptions options)
        {
            this.queueName = queueName;
            this.logger = logger;
            this.options = options;
        }

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    consumer.Dispose();

                disposed = true;
            }
        }

        #endregion

        #region IMessageQueueConsumer

        public event EventHandler<MessageQueueReceiveEventArgs> Received;

        public void Commit() => consumer.CommitAsync();

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                consumer.Poll(timeout);
            }
        }

        public void Reject() => consumer.Assign(consumer.Assignment);

        public void Subscribe(string queueTopic)
        {
            if (consumer == null)
            {
                lock (options)
                {
                    options.ConfigMapping["group.id"] = queueName;
                    var config = options.GetConfig();
                    consumer = new Consumer<Null, string>(config, null, deserializer);
                    consumer.OnMessage += (sender, e) =>
                    {
                        var eventArgs = new MessageQueueReceiveEventArgs(queueName, queueTopic, e.Value);
                        logger.LogInformation($"消息接收成功！ {eventArgs}");
                        Received?.Invoke(sender, eventArgs);
                    };
                    consumer.OnConsumeError += (sender, e) => logger.LogError($"消息消费失败！ [[QueueName = {queueName}, QueueTopic = {e.Topic}, QueueMessage = {e.Deserialize<Null, string>(null, deserializer).Value}]]");
                    consumer.OnError += (sender, e) => logger.LogError($"服务连接错误！ 原因：{e}");
                }
            }
        }

        #endregion
    }
}
