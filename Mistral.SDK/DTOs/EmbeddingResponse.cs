using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    /// <summary>
    /// EmbeddingResponse
    /// </summary>
    public class EmbeddingResponse
    {
        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        /// <example>embd-aad6fc62b17349b192ef09225058bc45</example>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or Sets VarObject
        /// </summary>
        /// <example>list</example>
        [JsonPropertyName("object")]
        public string VarObject { get; set; }

        /// <summary>
        /// Gets or Sets Data
        /// </summary>
        /// <example>[{&quot;object&quot;:&quot;embedding&quot;,&quot;embedding&quot;:[0.1,0.2,0.3],&quot;index&quot;:0},{&quot;object&quot;:&quot;embedding&quot;,&quot;embedding&quot;:[0.4,0.5,0.6],&quot;index&quot;:1}]</example>
        [JsonPropertyName("data")]
        public List<EmbeddingResult> Data { get; set; }

        /// <summary>
        /// Gets or Sets Model
        /// </summary>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// Gets or Sets Usage
        /// </summary>
        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }
    }
}
