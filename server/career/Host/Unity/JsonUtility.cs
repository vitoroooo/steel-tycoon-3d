using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace UnityEngine
{
    /// <summary>Stands in for Unity's JsonUtility on the server, so the game's own save code runs unchanged:
    /// public fields only, no properties, numbers as written.</summary>
    public static class JsonUtility
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            IncludeFields = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver { Modifiers = { FieldsOnly } }
        };

        public static string ToJson(object value) => JsonSerializer.Serialize(value, value.GetType(), Options);
        public static T FromJson<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
        public static object FromJson(string json, Type type) => JsonSerializer.Deserialize(json, type, Options);

        private static void FieldsOnly(JsonTypeInfo type)
        {
            if (type.Kind != JsonTypeInfoKind.Object) return;
            foreach (JsonPropertyInfo member in type.Properties.ToArray())
                if (!(member.AttributeProvider is FieldInfo field) || field.IsInitOnly || field.IsStatic)
                    type.Properties.Remove(member);
        }
    }
}
