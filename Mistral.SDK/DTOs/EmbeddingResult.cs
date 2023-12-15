using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    /// <summary>
    /// EmbeddingResult
    /// </summary>
    public class EmbeddingResult
    {
        /// <summary>
        /// Gets or Sets VarObject
        /// </summary>
        /// <example>embedding</example>
        [JsonPropertyName("object")]
        public string VarObject { get; set; }

        /// <summary>
        /// Gets or Sets Embedding
        /// </summary>
        /// <example>[0.1,0.2,0.3]</example>
        [JsonPropertyName("embedding")]
        public List<decimal> Embedding { get; set; }

        /// <summary>
        /// Gets or Sets Index
        /// </summary>
        /// <example>0</example>
        [JsonPropertyName("index")]
        public int Index { get; set; }
    }
}
