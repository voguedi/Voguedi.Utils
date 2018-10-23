using System;
using RabbitMQ.Client;

namespace Voguedi.Utils.RabbitMQ
{
    public interface IRabbitMQChannelPool : IDisposable
    {
        #region Methods

        IModel Pull();

        bool Push(IModel channel);

        #endregion
    }
}
