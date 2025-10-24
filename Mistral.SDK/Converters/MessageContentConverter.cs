using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mistral.SDK.Converters
{
    /// <summary>
    /// Custom JSON converter for message content that handles both string and array formats.
    /// Mistral thinking models return content as an array of content objects, while regular models return a simple string.
    /// This converter serializes array content to a JSON string to maintain backward compatibility.
    /// </summary>
    public class MessageContentConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    // Regular content is a simple string
                    return reader.GetString();

                case JsonTokenType.StartArray:
                    // Thinking model content is an array - serialize it to JSON string
                    using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                    {
                        return JsonSerializer.Serialize(doc.RootElement, options);
                    }

                case JsonTokenType.Null:
                    return null;

                default:
                    throw new JsonException($"Unexpected token type '{reader.TokenType}' when parsing message content. Expected String or StartArray.");
            }
        }
        
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value);
            }
        }
    }
}
