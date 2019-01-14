using System;

namespace Voguedi.Utils
{
    // https://github.com/dotnetcore/CAP/blob/develop/src/DotNetCore.CAP/Infrastructure/SnowflakeId.cs
    public class SnowflakeId
    {
        #region Private Fields
        
        const int workerIdBits = 5;
        const int datacenterIdBits = 5;
        const int sequenceBits = 12;
        const long maxWorkerId = -1L ^ (-1L << workerIdBits);
        const long maxDatacenterId = -1L ^ (-1L << datacenterIdBits);
        const int workerIdShift = sequenceBits;
        const int datacenterIdShift = sequenceBits + workerIdBits;
        const long sequenceMask = -1L ^ (-1L << sequenceBits);

        static readonly object staticSync = new object();

        readonly object sync = new object();

        static SnowflakeId instance;

        long lastTimestamp = -1L;

        #endregion

        #region Public Fields

        public const long Twepoch = 1288834974657L;
        public const int TimestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits;

        #endregion

        #region Public Properties

        public long WorkerId { get; protected set; }

        public long DatacenterId { get; protected set; }

        public long Sequence { get; internal set; }

        public static SnowflakeId Instance
        {
            get
            {
                lock (staticSync)
                {
                    if (instance != null)
                        return instance;

                    var random = new Random();
                    var workerId = random.Next((int)maxWorkerId);
                    var datacenterId = random.Next((int)maxDatacenterId);
                    return instance = new SnowflakeId(workerId, datacenterId);
                }
            }
        }

        #endregion

        #region Ctors

        public SnowflakeId(long workerId, long datacenterId, long sequence = 0L)
        {
            // sanity check for workerId
            if (workerId > maxWorkerId || workerId < 0)
                throw new ArgumentException(nameof(workerId), $"worker Id can't be greater than {maxWorkerId} or less than 0");

            if (datacenterId > maxDatacenterId || datacenterId < 0)
                throw new ArgumentException(nameof(datacenterId), $"datacenter Id can't be greater than {maxDatacenterId} or less than 0");

            WorkerId = workerId;
            DatacenterId = datacenterId;
            Sequence = sequence;
        }

        #endregion

        #region Protected Methods

        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) timestamp = TimeGen();
            return timestamp;
        }

        protected virtual long TimeGen()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        #endregion

        #region Public Methods

        public virtual long NewId()
        {
            lock (sync)
            {
                var timestamp = TimeGen();

                if (timestamp < lastTimestamp)
                    throw new Exception($"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {lastTimestamp - timestamp} milliseconds");

                if (lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & sequenceMask;

                    if (Sequence == 0)
                        timestamp = TilNextMillis(lastTimestamp);
                }
                else
                    Sequence = 0;

                lastTimestamp = timestamp;
                var id = ((timestamp - Twepoch) << TimestampLeftShift) |
                         (DatacenterId << datacenterIdShift) |
                         (WorkerId << workerIdShift) | Sequence;
                return id;
            }
        }

        #endregion
    }
}
