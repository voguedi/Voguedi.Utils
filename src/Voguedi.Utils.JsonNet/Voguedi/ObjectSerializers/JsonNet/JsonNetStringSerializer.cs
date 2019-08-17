using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Voguedi.ObjectSerializers.JsonNet
{
    class JsonNetStringSerializer : StringSerializer
    {
        #region Private Class

        class ContractResolver : DefaultContractResolver
        {
            #region Protected Methods

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jsonProperty = base.CreateProperty(member, memberSerialization);

                if (jsonProperty.Writable)
                    return jsonProperty;

                if (member is PropertyInfo propertyInfo)
                    jsonProperty.Writable = propertyInfo.GetSetMethod(true) != null;

                return jsonProperty;
            }

            #endregion
        }

        #endregion

        #region Private Fields

        readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new ContractResolver(),
            Converters = new List<JsonConverter> { new IsoDateTimeConverter() }
        };

        #endregion

        #region StringSerializer

        public override object Deserialize(Type objType, string objContent) => JsonConvert.DeserializeObject(objContent, objType, settings);

        public override TObject Deserialize<TObject>(string objContent) => JsonConvert.DeserializeObject<TObject>(objContent, settings);

        public override string Serialize(Type objType, object obj) => JsonConvert.SerializeObject(obj, objType, settings);

        #endregion
    }
}
