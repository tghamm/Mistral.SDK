using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using Mistral.SDK.Converters;

namespace Mistral.SDK.DTOs
{
    [JsonConverter(typeof(ToolChoiceTypeConverter))]
    public enum ToolChoiceType
    {
        [EnumMember(Value = "auto")]
        Auto,
        [EnumMember(Value = "any")]
        Any,
        [EnumMember(Value = "none")]
        none
    }
}
