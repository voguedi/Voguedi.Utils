using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Voguedi.ObjectSerializing
{
    public class BinaryObjectSerializer : IBinaryObjectSerializer
    {
        #region Private Fields

        readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        #endregion

        #region IObjectSerializer

        public virtual byte[] Serialize(Type type, object obj)
        {
            if (obj == null)
                return null;

            using (var stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public virtual byte[] Serialize<TObject>(TObject obj) where TObject : class => Serialize(typeof(TObject), obj);

        public virtual object Deserialize(byte[] content, Type type)
        {
            if (content == null)
                return null;

            using (var stream = new MemoryStream(content))
                return binaryFormatter.Deserialize(stream);
        }

        public virtual TObject Deserialize<TObject>(byte[] content) where TObject : class => (TObject)Deserialize(content, typeof(TObject));

        #endregion
    }
}
