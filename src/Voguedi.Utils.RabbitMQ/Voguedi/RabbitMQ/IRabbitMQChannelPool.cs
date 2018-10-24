using System;
using RabbitMQ.Client;

namespace Voguedi.RabbitMQ
{
    public interface IRabbitMQChannelPool : IDisposable
    {
        #region Methods

        IModel Pull();

        bool Push(IModel channel);

        #endregion
    }
}
