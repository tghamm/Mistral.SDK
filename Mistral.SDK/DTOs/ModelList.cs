using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class ModelList
    {
        /// <summary>
        /// Gets or Sets VarObject
        /// </summary>
        [JsonPropertyName("object")]
        public string VarObject { get; set; }

        /// <summary>
        /// Gets or Sets Data
        /// </summary>
        [JsonPropertyName("data")]
        public List<Model> Data { get; set; }
    }
}
