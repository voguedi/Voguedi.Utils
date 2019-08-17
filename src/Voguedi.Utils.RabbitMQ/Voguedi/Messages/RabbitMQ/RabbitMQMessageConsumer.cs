using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Voguedi.RabbitMQ;

namespace Voguedi.Messages.RabbitMQ
{
    class RabbitMQMessageConsumer : DisposableObject, IMessageConsumer
    {
        #region Private Fields

        readonly string group;
        readonly IRabbitMQConnectionProvider connectionProvider;
        readonly RabbitMQOptions options;
        readonly string exchangeName;
        readonly SemaphoreSlim connectionLock;
        IConnection connection;
        IModel channel;
        ulong deliveryTag;
        bool disposed;

        #endregion

        #region Ctors

        public RabbitMQMessageConsumer(string group, IRabbitMQConnectionProvider connectionProvider, RabbitMQOptions options)
        {
            this.group = group;
            this.connectionProvider = connectionProvider;
            this.options = options;
            exchangeName = options.ExchangeName;
            connectionLock = new SemaphoreSlim(1, 1);
        }

        #endregion

        #region Private Methods

        void TryConnect()
        {
            if (connection != null)
                return;

            connectionLock.Wait();

            try
            {
                if (connection == null)
                {
                    connection = connectionProvider.Get();
                    channel = connection.CreateModel();
                    channel.ExchangeDeclare(exchangeName, RabbitMQOptions.ExchangeType, true);
                    channel.QueueDeclare(group, true, false, false, new Dictionary<string, object> { { "x-message-ttl", options.QueueMessageExpires } });
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
                channel?.Dispose();
                connection?.Dispose();
                connectionLock.Dispose();
                disposed = true;
            }
        }

        #endregion

        #region IMessageConsumer

        public event EventHandler<MessageConsumerReceivedEventArgs> Received;
        public event EventHandler<MessageConsumerLoggedEventArgs> Logged;

        public void Commit() => channel.BasicAck(deliveryTag, false);

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            TryConnect();

            var consumer = new EventingBasicConsumer(channel);
            consumer.ConsumerCancelled += (sender, e) =>
                Logged?.Invoke(sender, new MessageConsumerLoggedEventArgs($"RabbitMQ consumer cancelled! [Group = {group}, ConsumerTag = {e.ConsumerTag}]"));
            consumer.Received += (sender, e) =>
            {
                deliveryTag = e.DeliveryTag;
                Received?.Invoke(sender, new MessageConsumerReceivedEventArgs(group, e.RoutingKey, Encoding.UTF8.GetString(e.Body)));
            };
            consumer.Registered += (sender, e) =>
                Logged?.Invoke(sender, new MessageConsumerLoggedEventArgs($"RabbitMQ consumer registered! [Group = {group}, ConsumerTag = {e.ConsumerTag}]"));
            consumer.Shutdown += (sender, e) =>
                Logged?.Invoke(sender, new MessageConsumerLoggedEventArgs($"RabbitMQ consumer shutdown! [Group = {group}, Reason = {e.ReplyText}]"));
            consumer.Unregistered += (sender, e) =>
                Logged?.Invoke(sender, new MessageConsumerLoggedEventArgs($"RabbitMQ consumer unregistered! [Group = {group}, ConsumerTag = {e.ConsumerTag}]"));
            channel.BasicConsume(group, false, consumer);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                cancellationToken.WaitHandle.WaitOne(timeout);
            }
        }

        public void Reject() => channel.BasicReject(deliveryTag, true);

        public void Subscribe(IEnumerable<string> topics)
        {
            if (topics == null)
                throw new ArgumentNullException(nameof(topics));

            TryConnect();

            foreach (var topic in topics)
                channel.QueueBind(group, exchangeName, topic);
        }

        #endregion
    }
}
