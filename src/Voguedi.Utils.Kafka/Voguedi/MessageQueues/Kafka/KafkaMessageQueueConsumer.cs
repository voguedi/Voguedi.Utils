using System;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Voguedi.Infrastructure;

namespace Voguedi.MessageQueues.Kafka
{
    class KafkaMessageQueueConsumer : DisposableObject, IMessageQueueConsumer
    {
        #region Private Fields

        readonly string queueName;
        readonly KafkaOptions options;
        readonly StringDeserializer deserializer = new StringDeserializer(Encoding.UTF8);
        Consumer<Null, string> consumer;
        bool disposed;

        #endregion

        #region Ctors

        public KafkaMessageQueueConsumer(string queueName, KafkaOptions options)
        {
            this.queueName = queueName;
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
                    var receivingMessage = new MessageQueueReceivedEventArgs(queueName, e.Topic, e.Value);
                    Received?.Invoke(sender, receivingMessage);
                };
                consumer.OnConsumeError += (sender, e) => Logged?.Invoke(sender, new MessageQueueLoggedEventArgs(MessageQueueLogType.KafkaOnConsumeError, e.Error.ToString()));
                consumer.OnError += (sender, e) => Logged?.Invoke(sender, new MessageQueueLoggedEventArgs(MessageQueueLogType.KafkaOnError, e.ToString()));
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

        #region IMessageQueueConsumer

        public event EventHandler<MessageQueueReceivedEventArgs> Received;
        public event EventHandler<MessageQueueLoggedEventArgs> Logged;

        public string BrokerAddress => options.Servers;

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
