using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class Page
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("markdown")]
        public string Markdown { get; set; }

        [JsonPropertyName("images")]
        public List<Image> Images { get; set; }

        [JsonPropertyName("dimensions")]
        public Dimensions Dimensions { get; set; }
    }
}
