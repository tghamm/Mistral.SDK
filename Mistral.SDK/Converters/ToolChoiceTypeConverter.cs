using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Converters
{
    public class ToolChoiceTypeConverter : JsonConverter<ToolChoiceType>
    {
        public override ToolChoiceType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            return value switch
            {
                "auto" => ToolChoiceType.Auto,
                "any" => ToolChoiceType.Any,
                "none" => ToolChoiceType.none,
                _ => throw new JsonException($"Unknown tool choice type: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, ToolChoiceType value, JsonSerializerOptions options)
        {
            string roleString = value switch
            {
                ToolChoiceType.Auto => "auto",
                ToolChoiceType.Any => "any",
                ToolChoiceType.none => "none",
                _ => throw new InvalidOperationException("Invalid tool choice type")
            };
            writer.WriteStringValue(roleString);
        }
    }
}
