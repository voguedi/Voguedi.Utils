using System;

namespace Voguedi.ObjectSerializing
{
    public interface IBinaryObjectSerializer
    {
        #region Methods

        byte[] Serialize(Type type, object obj);

        byte[] Serialize<TObject>(TObject obj) where TObject : class;

        object Deserialize(byte[] content, Type type);

        TObject Deserialize<TObject>(byte[] content) where TObject : class;

        #endregion
    }
}
