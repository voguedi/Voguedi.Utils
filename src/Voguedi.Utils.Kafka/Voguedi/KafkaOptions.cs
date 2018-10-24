using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Voguedi
{
    public class KafkaOptions
    {
        #region Private Fields

        IEnumerable<KeyValuePair<string, object>> config;

        #endregion

        #region Public Fields

        public readonly ConcurrentDictionary<string, object> ConfigMapping = new ConcurrentDictionary<string, object>();

        #endregion

        #region Public Properties

        public string Servers { get; set; }

        public int ProducerPoolSize { get; set; } = 10;

        #endregion

        #region Public Methods

        public IEnumerable<KeyValuePair<string, object>> GetConfig()
        {
            if (config == null)
            {
                if (string.IsNullOrWhiteSpace(Servers))
                    throw new ArgumentNullException(nameof(Servers));

                ConfigMapping["bootstrap.servers"] = Servers;
                ConfigMapping["queue.buffering.max.ms"] = "10";
                ConfigMapping["socket.blocking.max.ms"] = "10";
                ConfigMapping["enable.auto.commit"] = "false";
                ConfigMapping["log.connection.close"] = "false";
                ConfigMapping["request.timeout.ms"] = "3000";
                ConfigMapping["message.timeout.ms"] = "5000";
                config = ConfigMapping.AsEnumerable();
            }

            return config;
        }

        #endregion
    }
}
