using System;
using System.IO;
using ProtoBuf;

namespace Voguedi.ObjectSerializers.Protobuf
{
    class ProtobufBinarySerializer : BinarySerializer
    {
        #region BinarySerializer

        public override object Deserialize(Type objType, ArraySegment<byte> objContent)
        {
            using (var stream = new MemoryStream(objContent.Array))
                return Serializer.Deserialize(objType, stream);
        }

        public override TObject Deserialize<TObject>(ArraySegment<byte> objContent)
        {
            using (var stream = new MemoryStream(objContent.Array))
                return Serializer.Deserialize<TObject>(stream);
        }

        public override ArraySegment<byte> Serialize(Type objType, object obj)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.NonGeneric.Serialize(stream, obj);
                return new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }

        public override ArraySegment<byte> Serialize<TObject>(TObject obj)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, obj);
                return new ArraySegment<byte>(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }

        #endregion
    }
}
