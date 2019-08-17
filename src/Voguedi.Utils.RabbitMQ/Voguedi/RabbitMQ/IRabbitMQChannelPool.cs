using System;
using RabbitMQ.Client;

namespace Voguedi.RabbitMQ
{
    public interface IRabbitMQChannelPool : IDisposable
    {
        #region Methods

        IModel Get();

        bool TryReturn(IModel channel);

        #endregion
    }
}
