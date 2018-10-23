using RabbitMQ.Client;

namespace Voguedi.Utils.RabbitMQ
{
    public interface IRabbitMQConnectionPool
    {
        #region Methods

        IConnection Pull();

        #endregion
    }
}
