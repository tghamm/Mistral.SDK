using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class Function
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("parameters")]
        public Parameter Parameters { get; set; }
    }


    /// <summary>
    /// Parameter Class
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Type of the Schema, default is object
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "object";

        /// <summary>
        /// Properties of the Schema
        /// </summary>
        [JsonPropertyName("properties")]
        public Dictionary<string, Property> Properties { get; set; }

        /// <summary>
        /// Required Properties
        /// </summary>
        [JsonPropertyName("required")]
        public IList<string> Required { get; set; }
    }
    /// <summary>
    /// Serializable Tool Class
    /// </summary>
    public class Tool
    {
        /// <summary>
        /// Tool Type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "function";
        
        /// <summary>
        /// Tool Input Schema
        /// </summary>
        [JsonPropertyName("function")]
        public Function Function { get; set; }
    }

    /// <summary>
    /// Property Definition Class
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Property Type
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Enum Values as Strings (if applicable)
        /// </summary>
        [JsonPropertyName("enum")]
        public string[] Enum { get; set; }

        /// <summary>
        /// Description of the Property
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
