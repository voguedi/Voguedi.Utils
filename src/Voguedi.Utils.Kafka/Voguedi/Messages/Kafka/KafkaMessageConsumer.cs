using System;
using System.Collections.Generic;
using System.Threading;
using Confluent.Kafka;

namespace Voguedi.Messages.Kafka
{
    class KafkaMessageConsumer : DisposableObject, IMessageConsumer
    {
        #region Private Fields

        readonly string group;
        readonly KafkaOptions options;
        readonly SemaphoreSlim connectionLock;
        IConsumer<Null, string> consumer;
        bool disposed;

        #endregion

        #region Ctors

        public KafkaMessageConsumer(string group, KafkaOptions options)
        {
            this.group = group;
            this.options = options;
            connectionLock = new SemaphoreSlim(1, 1);
        }

        #endregion

        #region Private Methods

        void TryConnect()
        {
            if (consumer != null)
                return;

            connectionLock.Wait();

            try
            {
                if (consumer == null)
                {
                    options.MainConfig["group.id"] = group;
                    options.MainConfig["auto.offset.reset"] = "earliest";
                    consumer = new ConsumerBuilder<Null, string>(options.GetConfig())
                        .SetErrorHandler((c, e) => Logged?.Invoke(null, new MessageConsumerLoggedEventArgs($"Kafka connection error! [Group = {group}, Reason = {e.Reason}]")))
                        .Build();
                }
            }
            finally
            {
                connectionLock.Release();
            }
        }

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                consumer?.Dispose();
                connectionLock.Dispose();
                disposed = true;
            }
        }

        #endregion

        #region IMessageConsumer

        public event EventHandler<MessageConsumerReceivedEventArgs> Received;
        public event EventHandler<MessageConsumerLoggedEventArgs> Logged;

        public void Commit() => consumer.Commit();

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            TryConnect();

            while (true)
            {
                var result = consumer.Consume(cancellationToken);

                if (result.IsPartitionEOF || result.Value == null)
                    continue;

                Received?.Invoke(result, new MessageConsumerReceivedEventArgs(group, result.Topic, result.Value));
            }
        }

        public void Reject() => consumer.Assign(consumer.Assignment);

        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null)
                throw new ArgumentNullException(nameof(topics));

            TryConnect();
            consumer.Subscribe(topics);
        }

        #endregion
    }
}
