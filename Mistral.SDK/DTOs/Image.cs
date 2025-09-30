


using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class Image
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("top_left_x")]
        public int TopLeftX { get; set; }

        [JsonPropertyName("top_left_y")]
        public int TopLeftY { get; set; }

        [JsonPropertyName("bottom_right_x")]
        public int BottomRightX { get; set; }

        [JsonPropertyName("bottom_right_y")]
        public int BottomRightY { get; set; }

        [JsonPropertyName("image_base64")]
        public string ImageBase64 { get; set; }
    }
}
