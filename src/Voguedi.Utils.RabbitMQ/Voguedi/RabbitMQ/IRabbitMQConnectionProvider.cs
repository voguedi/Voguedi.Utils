using System;
using RabbitMQ.Client;

namespace Voguedi.RabbitMQ
{
    public interface IRabbitMQConnectionProvider : IDisposable
    {
        #region Methods

        IConnection Get();

        #endregion
    }
}
