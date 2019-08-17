using System;

namespace Voguedi.ObjectSerializers
{
    public abstract class BinarySerializer : IBinarySerializer
    {
        #region IBinarySerializer

        public abstract object Deserialize(Type objType, ArraySegment<byte> objContent);

        public virtual TObject Deserialize<TObject>(ArraySegment<byte> objContent) => (TObject)Deserialize(typeof(TObject), objContent);

        public abstract ArraySegment<byte> Serialize(Type objType, object obj);

        public virtual ArraySegment<byte> Serialize<TObject>(TObject obj) => Serialize(typeof(TObject), obj);

        #endregion
    }
}
