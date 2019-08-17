using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Voguedi
{
    public class KafkaOptions
    {
        #region Public Fields

        public readonly ConcurrentDictionary<string, string> MainConfig = new ConcurrentDictionary<string, string>();

        #endregion

        #region Private Fields

        IEnumerable<KeyValuePair<string, string>> config;

        #endregion

        #region Public Properties

        public string Servers { get; set; }

        public int ProducerPoolSize { get; set; } = 10;

        #endregion

        #region Internal Methods

        internal IEnumerable<KeyValuePair<string, string>> GetConfig()
        {
            if (config == null)
            {
                if (string.IsNullOrWhiteSpace(Servers))
                    throw new ArgumentNullException(nameof(Servers));

                MainConfig["bootstrap.servers"] = Servers;
                MainConfig["queue.buffering.max.ms"] = "10";
                MainConfig["enable.auto.commit"] = "false";
                MainConfig["log.connection.close"] = "false";
                MainConfig["request.timeout.ms"] = "3000";
                MainConfig["message.timeout.ms"] = "5000";
                config = MainConfig.AsEnumerable();
            }

            return config;
        }

        #endregion
    }
}
