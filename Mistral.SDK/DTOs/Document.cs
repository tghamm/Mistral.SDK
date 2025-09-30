

using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class Document
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }


        [JsonPropertyName("document_url")]
        public string DocumentUrl { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }
}
