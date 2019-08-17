namespace Voguedi
{
    public class RabbitMQOptions
    {
        #region Public Fields

        public const string ExchangeType = "topic";

        #endregion

        #region Public Properties

        public string HostName { get; set; } = "localhost";

        public string VirtualHost { get; set; } = "/";

        public int Port { get; set; } = -1;

        public string UserName { get; set; } = "guest";

        public string Password { get; set; } = "guest";

        public int ChannelPoolSize { get; set; } = 10;

        public string ExchangeName { get; set; } = "voguedi.default";

        public int QueueMessageExpires { get; set; } = 864000000;

        #endregion
    }
}
