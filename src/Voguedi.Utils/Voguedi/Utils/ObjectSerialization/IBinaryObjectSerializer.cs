using System;
using Voguedi.Utils.DependencyInjection;

namespace Voguedi.Utils.ObjectSerialization
{
    public interface IBinaryObjectSerializer : ITransientDependency
    {
        #region Methods

        byte[] Serialize(Type type, object obj);

        byte[] Serialize<TObject>(TObject obj) where TObject : class;

        object Deserialize(byte[] content, Type type);

        TObject Deserialize<TObject>(byte[] content) where TObject : class;

        #endregion
    }
}
