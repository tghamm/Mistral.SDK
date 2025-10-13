using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class OCRRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("document")]
        public Document Document { get; set; }

        [JsonPropertyName("include_image_base64")]
        public bool IncludeImageBase64 { get; set; }
    }
}
