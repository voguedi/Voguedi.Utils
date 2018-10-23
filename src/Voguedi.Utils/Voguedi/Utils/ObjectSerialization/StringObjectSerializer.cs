using System;

namespace Voguedi.Utils.ObjectSerialization
{
    public abstract class StringObjectSerializer : IStringObjectSerializer
    {
        #region IStringObjectSerializer

        public abstract object Deserialize(string content, Type type);

        public virtual TObject Deserialize<TObject>(string content) where TObject : class => (TObject)Deserialize(content, typeof(TObject));

        public abstract string Serialize(Type type, object obj);

        public virtual string Serialize<TObject>(TObject obj) where TObject : class => Serialize(typeof(TObject), obj);

        #endregion
    }
}
