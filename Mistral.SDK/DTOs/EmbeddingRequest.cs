using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using Mistral.SDK.Converters;

namespace Mistral.SDK.DTOs
{
    public class EmbeddingRequest
    {
        /// <summary>
        /// The format of the output data. 
        /// </summary>
        /// <value>The format of the output data. </value>
        [JsonConverter(typeof(JsonPropertyNameEnumConverter<EncodingFormatEnum>))]
        public enum EncodingFormatEnum
        {
            /// <summary>
            /// Enum Float for value: float
            /// </summary>
            [JsonPropertyName("float")]
            Float = 1
        }

        /// <summary>
        /// The format of the output data. 
        /// </summary>
        /// <value>The format of the output data. </value>
        /// <example>float</example>
        [JsonPropertyName("encoding_format")]
        public EncodingFormatEnum? EncodingFormat { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingRequest" /> class.
        /// </summary>
        /// <param name="model">The ID of the model to use for this request. .</param>
        /// <param name="input">The list of strings to embed. .</param>
        /// <param name="encodingFormat">The format of the output data. .</param>
        public EmbeddingRequest(string model = default(string), List<string> input = default(List<string>), EncodingFormatEnum? encodingFormat = default(EncodingFormatEnum?))
        {
            this.Model = model;
            this.Input = input;
            this.EncodingFormat = encodingFormat;
        }

        /// <summary>
        /// The ID of the model to use for this request. 
        /// </summary>
        /// <value>The ID of the model to use for this request. </value>
        /// <example>mistral-embed</example>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// The list of strings to embed. 
        /// </summary>
        /// <value>The list of strings to embed. </value>
        /// <example>[&quot;Hello&quot;,&quot;world&quot;]</example>
        [JsonPropertyName("input")]
        public List<string> Input { get; set; }
    }
}
