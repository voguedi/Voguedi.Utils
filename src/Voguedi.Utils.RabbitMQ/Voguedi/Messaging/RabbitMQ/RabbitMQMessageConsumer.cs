using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Voguedi.DisposableObjects;
using Voguedi.RabbitMQ;

namespace Voguedi.Messaging.RabbitMQ
{
    class RabbitMQMessageConsumer : DisposableObject, IMessageConsumer
    {
        #region Private Fields

        readonly string queueName;
        readonly IRabbitMQConnectionPool connectionPool;
        readonly ILogger logger;
        readonly string exchangeName;
        IConnection connection;
        IModel channel;
        ulong deliveryTag;
        bool disposed = false;

        #endregion

        #region Ctors

        public RabbitMQMessageConsumer(string queueName, IRabbitMQConnectionPool connectionPool, ILogger<RabbitMQMessageConsumer> logger, RabbitMQOptions options)
        {
            this.queueName = queueName;
            this.connectionPool = connectionPool;
            this.logger = logger;
            exchangeName = options.ExchangeName;
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

        #region IMessageConsumer

        public event EventHandler<ReceivingMessage> Received;

        public void Commit() => channel.BasicAck(deliveryTag, false);

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                deliveryTag = e.DeliveryTag;
                var receivingMessage = new ReceivingMessage(queueName, e.RoutingKey, Encoding.UTF8.GetString(e.Body));
                logger.LogInformation($"消息接收成功！ {receivingMessage}");
                Received?.Invoke(sender, receivingMessage);
            };
            consumer.ConsumerCancelled += (sender, e) => logger.LogError($"消息消费取消！原因：{e.ConsumerTag}");
            consumer.Registered += (sender, e) => logger.LogInformation($"消息消费者注册成功！原因：{e.ConsumerTag}");
            consumer.Shutdown += (sender, e) => logger.LogError($"消息消费者已关闭！原因：{e.ReplyText}");
            consumer.Unregistered += (sender, e) => logger.LogError($"消息消费者未注册！原因：{e.ConsumerTag}");
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
