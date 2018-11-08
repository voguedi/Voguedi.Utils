using System;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using Voguedi.DisposableObjects;

namespace Voguedi.Messaging.Kafka
{
    class KafkaMessageConsumer : DisposableObject, IMessageConsumer
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

        public KafkaMessageConsumer(string queueName, ILogger<KafkaMessageConsumer> logger, KafkaOptions options)
        {
            this.queueName = queueName;
            this.logger = logger;
            this.options = options;
        }

        #endregion

        #region Private Methods

        void InitializeConsumer()
        {
            lock (options)
            {
                options.ConfigMapping["group.id"] = queueName;
                var config = options.GetConfig();
                consumer = new Consumer<Null, string>(config, null, deserializer);
                consumer.OnMessage += (sender, e) =>
                {
                    var receivingMessage = new ReceivingMessage(queueName, e.Topic, e.Value);
                    logger.LogInformation($"消息接收成功！ {receivingMessage}");
                    Received?.Invoke(sender, receivingMessage);
                };
                consumer.OnConsumeError += (sender, e) => logger.LogError($"消息消费失败！ [[QueueName = {queueName}, QueueTopic = {e.Topic}, QueueMessage = {e.Deserialize<Null, string>(null, deserializer).Value}]]");
                consumer.OnError += (sender, e) => logger.LogError($"服务连接错误！ 原因：{e}");
            }
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

        #region IMessageConsumer

        public event EventHandler<ReceivingMessage> Received;

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

        public void Subscribe(params string[] queueTopics)
        {
            if (queueTopics == null)
                throw new ArgumentNullException(nameof(queueTopics));

            if (consumer == null)
                InitializeConsumer();

            consumer.Subscribe(queueTopics);
        }

        #endregion
    }
}
