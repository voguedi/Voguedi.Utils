using System;

namespace Voguedi.ObjectSerializers
{
    public interface IBinarySerializer
    {
        #region Methods

        ArraySegment<byte> Serialize(Type objType, object obj);

        ArraySegment<byte> Serialize<TObject>(TObject obj);

        object Deserialize(Type objType, ArraySegment<byte> objContent);

        TObject Deserialize<TObject>(ArraySegment<byte> objContent);

        #endregion
    }
}
