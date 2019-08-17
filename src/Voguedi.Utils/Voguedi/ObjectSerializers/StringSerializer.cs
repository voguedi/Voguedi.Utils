using System;

namespace Voguedi.ObjectSerializers
{
    public abstract class StringSerializer : IStringSerializer
    {
        #region IStringSerializer

        public abstract object Deserialize(Type objType, string objContent);

        public virtual TObject Deserialize<TObject>(string objContent) where TObject : class => (TObject)Deserialize(typeof(TObject), objContent);

        public abstract string Serialize(Type objType, object obj);

        public virtual string Serialize<TObject>(TObject obj) where TObject : class => Serialize(typeof(TObject), obj);

        #endregion
    }
}
