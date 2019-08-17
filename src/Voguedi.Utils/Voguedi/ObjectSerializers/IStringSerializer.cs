using System;

namespace Voguedi.ObjectSerializers
{
    public interface IStringSerializer
    {
        #region Methods

        string Serialize(Type objType, object obj);

        string Serialize<TObject>(TObject obj) where TObject : class;

        object Deserialize(Type objType, string objContent);

        TObject Deserialize<TObject>(string objContent) where TObject : class;

        #endregion
    }
}
