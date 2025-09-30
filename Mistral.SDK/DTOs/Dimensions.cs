

using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class Dimensions
    {
        [JsonPropertyName("dpi")]
        public int Dpi { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}
