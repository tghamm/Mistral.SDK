using System.Collections.Generic;
using System.Text.Json.Serialization;
using Mistral.SDK.Converters;

namespace Mistral.SDK.DTOs
{
    public class ChatMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatMessage" /> class.
        /// </summary>
        /// <param name="role">role.</param>
        /// <param name="content">content.</param>
        public ChatMessage(RoleEnum? role = default(RoleEnum?), string content = default(string))
        {
            this.Role = role;
            this.Content = content;
        }

        public ChatMessage(Common.Function? toolCall, string content = default(string))
        {
            this.Role = RoleEnum.Tool;
            this.Name = toolCall.Name;
            this.ToolCallId = toolCall.Id;
            this.Content = content;
        }

        public ChatMessage()
        {

        }


        [JsonConverter(typeof(JsonPropertyNameEnumConverter<RoleEnum>))]
        public enum RoleEnum
        {
            /// <summary>
            /// Enum System for value: system
            /// </summary>
            [JsonPropertyName("system")]
            //[EnumMember(Value = "system")]
            System = 1,

            /// <summary>
            /// Enum User for value: user
            /// </summary>
            [JsonPropertyName("user")]
            //[EnumMember(Value = "user")]
            User = 2,

            /// <summary>
            /// Enum Assistant for value: assistant
            /// </summary>
            [JsonPropertyName("assistant")]
            //[EnumMember(Value = "assistant")]
            Assistant = 3,

            [JsonPropertyName("tool")]
            Tool = 4
        }

        /// <summary>
        /// Gets or Sets Role
        /// </summary>
        [JsonPropertyName("role")]
        public RoleEnum? Role { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// Gets or Sets Content
        /// </summary>
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("tool_calls")]
        public List<ToolCall> ToolCalls { get; set; }

        [JsonPropertyName("tool_call_id")]
        public string ToolCallId { get; set; }
    }
}
