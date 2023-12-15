using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;

namespace Mistral.SDK.Converters
{
    public class JsonPropertyNameEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();

            foreach (var field in typeToConvert.GetFields())
            {
                var attribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (attribute?.Name == value)
                {
                    return (T)Enum.Parse(typeToConvert, field.Name);
                }
            }

            throw new JsonException($"Unable to convert \"{value}\" to enum {typeToConvert}.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<JsonPropertyNameAttribute>();

            if (attribute != null)
            {
                writer.WriteStringValue(attribute.Name);
            }
            else
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}
