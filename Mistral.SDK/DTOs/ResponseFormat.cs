using Mistral.SDK.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
	public class ResponseFormat
	{
		[JsonConverter(typeof(JsonPropertyNameEnumConverter<ResponseFormatEnum>))]
		public enum ResponseFormatEnum
		{
			/// <summary>
			/// Enum json for value: json_object
			/// </summary>
			[JsonPropertyName("json_object")]
			JSON = 1
		}

		/// <summary>
		/// The output type
		/// </summary>
		[JsonPropertyName("type")]
		
		public ResponseFormatEnum Type { get; set; }
	}
}
