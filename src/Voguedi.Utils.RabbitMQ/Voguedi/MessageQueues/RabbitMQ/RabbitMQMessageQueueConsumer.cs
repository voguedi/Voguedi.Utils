using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Voguedi.Infrastructure;
using Voguedi.RabbitMQ;

namespace Voguedi.MessageQueues.RabbitMQ
{
    class RabbitMQMessageQueueConsumer : DisposableObject, IMessageQueueConsumer
    {
        #region Private Fields

        readonly string queueName;
        readonly string exchangeName;
        readonly IConnection connection;
        readonly IModel channel;
        ulong deliveryTag;
        bool disposed = false;

        #endregion

        #region Ctors

        public RabbitMQMessageQueueConsumer(string queueName, IRabbitMQConnectionPool connectionPool, RabbitMQOptions options)
        {
            this.queueName = queueName;
            exchangeName = options.ExchangeName;
            BrokerAddress = options.HostName;
            connection = connectionPool.Pull();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, options.ExchangeType, true);
            channel.QueueDeclare(queueName, true, false, false, new Dictionary<string, object> { { "x-message-ttl", options.MessageExpires } });
        }

        #endregion

        #region DisposableObject

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    channel.Dispose();
                    connection.Dispose();
                }

                disposed = true;
            }
        }

        #endregion

        #region IMessageQueueConsumer

        public event EventHandler<MessageQueueReceivedEventArgs> Received;
        public event EventHandler<MessageQueueLoggedEventArgs> Logged;

        public string BrokerAddress { get; }

        public void Commit() => channel.BasicAck(deliveryTag, false);

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                deliveryTag = e.DeliveryTag;
                var receivingMessage = new MessageQueueReceivedEventArgs(queueName, e.RoutingKey, Encoding.UTF8.GetString(e.Body));
                Received?.Invoke(sender, receivingMessage);
            };
            consumer.ConsumerCancelled += (sender, e) => Logged?.Invoke(sender, new MessageQueueLoggedEventArgs(MessageQueueLogType.RabbitMQConsumerCancelled, e.ConsumerTag));
            consumer.Registered += (sender, e) => Logged?.Invoke(sender, new MessageQueueLoggedEventArgs(MessageQueueLogType.RabbitMQRegistered, e.ConsumerTag));
            consumer.Shutdown += (sender, e) => Logged?.Invoke(sender, new MessageQueueLoggedEventArgs(MessageQueueLogType.RabbitMQShutdown, e.ReplyText));
            consumer.Unregistered += (sender, e) => Logged?.Invoke(sender, new MessageQueueLoggedEventArgs(MessageQueueLogType.RabbitMQUnregistered, e.ConsumerTag));
            channel.BasicConsume(queueName, false, consumer);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                cancellationToken.WaitHandle.WaitOne(timeout);
            }
        }

        public void Reject() => channel.BasicReject(deliveryTag, true);

        public void Subscribe(params string[] queueTopics)
        {
            if (queueTopics == null)
                throw new ArgumentNullException(nameof(queueTopics));

            foreach (var queueTopic in queueTopics)
                channel.QueueBind(queueName, exchangeName, queueTopic);
        }

        #endregion
    }
}
