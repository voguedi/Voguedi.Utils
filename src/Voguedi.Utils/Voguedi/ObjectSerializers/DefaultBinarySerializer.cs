using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Voguedi.ObjectSerializers
{
    public class DefaultBinarySerializer : BinarySerializer
    {
        #region BinarySerializer

        public override object Deserialize(Type objType, ArraySegment<byte> objContent)
        {
            using (var stream = new MemoryStream(objContent.Array, objContent.Offset, objContent.Count))
                return new BinaryFormatter().Deserialize(stream);
        }

        public override ArraySegment<byte> Serialize(Type objType, object obj)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, obj);
                return new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }

        #endregion
    }
}
