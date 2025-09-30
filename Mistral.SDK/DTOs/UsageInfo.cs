

using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class UsageInfo
    {
        [JsonPropertyName("pages_processed")]
        public int PagesProcessed { get; set; }

        [JsonPropertyName("doc_size_bytes")]
        public int? DocSizeBytes { get; set; }
    }
}
