using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Voguedi.IdentityGeneration
{
    public class ObjectIdentityGenerator
    {
        #region Private Fields

        readonly DateTime epochTimestamp;
        readonly int machineHashCode;
        readonly short processId;
        int randomNumber;

        #endregion

        #region Ctors

        public ObjectIdentityGenerator()
        {
            epochTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            machineHashCode = GetMachineHashCode();
            processId = (short)GerProcessId();
            randomNumber = new Random().Next();
        }

        #endregion

        #region Private Methods

        int GetMachineHashCode()
        {
            using (var md5 = MD5.Create())
            {
                var hashCode = md5.ComputeHash(Encoding.UTF8.GetBytes(Environment.MachineName));
                return (hashCode[0] << 16) + (hashCode[1] << 8) + hashCode[2];
            }
        }

        int GerProcessId() => Process.GetCurrentProcess().Id;

        int GetTimestamp(DateTime dateTime) => (int)Math.Floor((ToUtc(dateTime) - epochTimestamp).TotalSeconds);

        DateTime ToUtc(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
                return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

            if (dateTime == DateTime.MaxValue)
                return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);

            return dateTime.ToUniversalTime();
        }

        int GetSequenceNumber() => Interlocked.Increment(ref randomNumber) & 0x00ffffff;

        byte[] Generate(int timestamp)
        {
            var sequenceNumber = GetSequenceNumber();
            var bytes = new byte[12];
            bytes[0] = (byte)(timestamp >> 24);
            bytes[1] = (byte)(timestamp >> 16);
            bytes[2] = (byte)(timestamp >> 8);
            bytes[3] = (byte)(timestamp);
            bytes[4] = (byte)(machineHashCode >> 16);
            bytes[5] = (byte)(machineHashCode >> 8);
            bytes[6] = (byte)(machineHashCode);
            bytes[7] = (byte)(processId >> 8);
            bytes[8] = (byte)(processId);
            bytes[9] = (byte)(sequenceNumber >> 16);
            bytes[10] = (byte)(sequenceNumber >> 8);
            bytes[11] = (byte)(sequenceNumber);
            return bytes;
        }

        #endregion

        #region Public Methods

        public byte[] Generate() => Generate(GetTimestamp(DateTime.UtcNow));

        #endregion
    }
}
