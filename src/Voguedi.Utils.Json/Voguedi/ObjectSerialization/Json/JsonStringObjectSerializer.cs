using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Voguedi.ObjectSerializing.Json
{
    class JsonStringObjectSerializer : StringObjectSerializer
    {
        #region Private Class

        class JsonContractResolver : DefaultContractResolver
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
            ContractResolver = new JsonContractResolver(),
            Converters = new List<JsonConverter> { new IsoDateTimeConverter() }
        };

        #endregion

        #region IJsonStringObjectSerializer

        public override object Deserialize(string content, Type type) => JsonConvert.DeserializeObject(content, type, settings);

        public override TObject Deserialize<TObject>(string content) => JsonConvert.DeserializeObject<TObject>(content, settings);

        public override string Serialize(Type type, object obj) => JsonConvert.SerializeObject(obj, type, settings);

        #endregion
    }
}
