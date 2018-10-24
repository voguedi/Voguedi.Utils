using RabbitMQ.Client;

namespace Voguedi.RabbitMQ
{
    public interface IRabbitMQConnectionPool
    {
        #region Methods

        IConnection Pull();

        #endregion
    }
}
