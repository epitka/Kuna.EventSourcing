using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonProperty = System.Text.Json.JsonProperty;

namespace Senf.EventSourcing.Core.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings()
            {
                ContractResolver = new PrivateResolver(),
            };

        public class PrivateResolver : DefaultContractResolver
        {
            protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(
                MemberInfo member,
                MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (prop.Writable)
                {
                    return prop;
                }

                var property = member as PropertyInfo;

                if (property == null)
                {
                    return prop;
                }

                var hasPrivateSetter = property.GetSetMethod(true) != null;
                prop.Writable = hasPrivateSetter;

                return prop;
            }
        }

        // TODO: see if this can done faster using
        // https://www.codeproject.com/Articles/1111658/Fast-Deep-Copy-by-Expression-Trees-C-Sharp
        public static T DeepClone<T>(this T source)
            where T : class
        {
            if (source is null) return default!;

            var serialized = JsonConvert.SerializeObject(source, JsonSerializerSettings);

            return JsonConvert.DeserializeObject<T>(serialized, JsonSerializerSettings)!;
        }

        public static string AsJson<T>(this T source)
            where T : class
        {
            // Don't serialize a null object,
            // simply return the default for that object
            return source is null
                ? string.Empty
                : JsonConvert.SerializeObject(source);
        }
    }
}
