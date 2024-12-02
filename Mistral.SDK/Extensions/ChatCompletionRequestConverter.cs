using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Extensions
{
    public class ChatCompletionRequestConverter: JsonConverter<ChatCompletionRequest>
    {
        public override ChatCompletionRequest Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Implement the Read method if deserialization is needed
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ChatCompletionRequest value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var properties = typeof(ChatCompletionRequest).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
                    continue;

                var jsonPropertyName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? property.Name;
                var propertyValue = property.GetValue(value);

                if (options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingNull && propertyValue == null)
                    continue;

                writer.WritePropertyName(jsonPropertyName);
                JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);
            }

            writer.WriteEndObject();
        }
    }
}
