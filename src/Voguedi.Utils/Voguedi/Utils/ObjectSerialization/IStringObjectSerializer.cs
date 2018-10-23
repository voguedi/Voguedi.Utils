using System;

namespace Voguedi.Utils.ObjectSerialization
{
    public interface IStringObjectSerializer
    {
        #region Methods

        string Serialize(Type type, object obj);

        string Serialize<TObject>(TObject obj) where TObject : class;

        object Deserialize(string content, Type type);

        TObject Deserialize<TObject>(string content) where TObject : class;

        #endregion
    }
}
