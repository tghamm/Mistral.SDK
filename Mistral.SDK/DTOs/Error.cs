using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class Error
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or Sets Param
        /// </summary>
        [JsonPropertyName("param")]
        public string Param { get; set; }

        /// <summary>
        /// Gets or Sets Code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
