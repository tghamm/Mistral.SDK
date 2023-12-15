using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or Sets Error
        /// </summary>
        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }
}
