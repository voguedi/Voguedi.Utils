namespace Voguedi.Utils
{
    public class RabbitMQOptions
    {
        #region Public Properties

        public string HostName { get; set; } = "localhost";

        public string VirtualHost { get; set; } = "/";

        public int Port { get; set; } = -1;

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public int RequestedConnectionTimeout { get; set; } = 30000;

        public int SocketReadTimeout { get; set; } = 30000;

        public int SocketWriteTimeout { get; set; } = 30000;

        public int MessageExpires { get; set; } = 864000000;

        public int ChannelPoolSize { get; set; } = 15;

        public string ExchangeName { get; set; }

        public string ExchangeType { get; set; } = "topic";

        #endregion
    }
}
