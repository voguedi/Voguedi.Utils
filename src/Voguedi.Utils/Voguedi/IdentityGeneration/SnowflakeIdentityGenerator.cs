using System;

namespace Voguedi.IdentityGeneration
{
    public class SnowflakeIdentityGenerator
    {
        #region Private Fields

        const long twepoch = 1288834974657L;
        const int workerIdBits = 5;
        const int datacenterIdBits = 5;
        const int sequenceBits = 12;
        const long maxWorkerId = -1L ^ (-1L << workerIdBits);
        const long maxDatacenterId = -1L ^ (-1L << datacenterIdBits);
        const int workerIdShift = sequenceBits;
        const int datacenterIdShift = sequenceBits + workerIdBits;
        const int timestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits;
        const long sequenceMask = -1L ^ (-1L << sequenceBits);

        static readonly object syncStaticObj = new object();
        static SnowflakeIdentityGenerator instance;

        readonly object syncObj = new object();

        long lastTimestamp = -1L;

        #endregion

        #region Public Properties

        public long WorkerId { get; set; }

        public long DatacenterId { get; set; }

        public long Sequence { get; set; }

        public static SnowflakeIdentityGenerator Instance
        {
            get
            {
                lock (syncStaticObj)
                {
                    if (instance != null)
                        return instance;

                    var random = new Random();
                    var workerId = random.Next((int)maxWorkerId);
                    var datacenterId = random.Next((int)maxDatacenterId);
                    return instance = new SnowflakeIdentityGenerator(workerId, datacenterId);
                }
            }
        }

        #endregion

        #region Ctors

        public SnowflakeIdentityGenerator(long workerId, long datacenterId, long sequence = 0L)
        {
            if (workerId > maxWorkerId || workerId < 0)
                throw new ArgumentOutOfRangeException(nameof(workerId));

            if (datacenterId > maxDatacenterId || datacenterId < 0)
                throw new ArgumentOutOfRangeException(nameof(datacenterId));

            WorkerId = workerId;
            DatacenterId = datacenterId;
            Sequence = sequence;
        }

        #endregion

        #region Private Methods

        long GetTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        long GetNextTimestamp(long lastTimestamp)
        {
            var timestamp = GetTimestamp();

            while (timestamp <= lastTimestamp)
                timestamp = GetTimestamp();

            return timestamp;
        }

        #endregion

        #region Public Methods

        public long Generate()
        {
            lock (syncObj)
            {
                var timestamp = GetTimestamp();

                if (timestamp < lastTimestamp)
                    throw new Exception($"系统时间戳无效！ [LastTimestamp = {lastTimestamp}, CurrentTimestamp = {timestamp}]");

                if (lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & sequenceMask;

                    if (Sequence == 0)
                        timestamp = GetNextTimestamp(lastTimestamp);
                }
                else
                    Sequence = 0;

                lastTimestamp = timestamp;
                var id = ((timestamp - twepoch) << timestampLeftShift) |
                         (DatacenterId << datacenterIdShift) |
                         (WorkerId << workerIdShift) | Sequence;
                return id;
            }
        }

        #endregion
    }
}
