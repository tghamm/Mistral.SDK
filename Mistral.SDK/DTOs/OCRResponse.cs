using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class OCRResponse
    {
        [JsonPropertyName("pages")]
        public List<Page> Pages { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("usage_info")]
        public UsageInfo UsageInfo { get; set; }
    }
}
