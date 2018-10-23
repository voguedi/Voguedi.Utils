using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Voguedi.Utils.IdentityGeneration
{
    public class StringIdentityGenerator : IStringIdentityGenerator
    {
        #region Private Class

        class ObjectIdentityGenerator
        {
            #region Private Fields

            readonly DateTime epochDateTime;
            readonly int machineHashCode;
            readonly short processId;
            int randomNumber;

            #endregion

            #region Ctors

            public ObjectIdentityGenerator()
            {
                epochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
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

            int GetTimestamp(DateTime dateTime) => (int)Math.Floor((ToUtc(dateTime) - epochDateTime).TotalSeconds);

            DateTime ToUtc(DateTime dateTime)
            {
                if (dateTime == DateTime.MinValue)
                    return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
                else if (dateTime == DateTime.MaxValue)
                    return DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc);
                else
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

        #endregion

        #region Private Fields

        readonly ObjectIdentityGenerator generator = new ObjectIdentityGenerator();
        readonly uint[] lookup32 = Enumerable.Range(0, 256).Select(item =>
        {
            var str = item.ToString("x2");
            return str[0] + ((uint)str[1] << 16);
        }).ToArray();

        #endregion

        #region Public Properties

        public static StringIdentityGenerator Instance => new StringIdentityGenerator();

        #endregion

        #region IStringIdentityGenerator

        public string Generate()
        {
            var objectId = generator.Generate();
            var chars = new char[24];

            for (int i = 0; i < 12; i++)
            {
                var val = lookup32[objectId[i]];
                chars[2 * i] = (char)val;
                chars[2 * i + 1] = (char)(val >> 16);
            }

            return new string(chars);
        }

        #endregion
    }
}
