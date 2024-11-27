using System.Text.Json;
using System.Text.Json.Serialization;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Converters;

public static class MistalSdkJsonOption
{

#if NET8_0_OR_GREATER
    public static readonly JsonSerializerOptions Options = JsonContext.Default.Options;
#else
    public static readonly JsonSerializerOptions Options = null;
#endif
}

#if NET8_0_OR_GREATER

[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(ChatCompletionRequest))]
[JsonSerializable(typeof(ChatCompletionResponse))]
[JsonSerializable(typeof(ChatMessage))]
[JsonSerializable(typeof(Choice))]
[JsonSerializable(typeof(EmbeddingRequest))]
[JsonSerializable(typeof(EmbeddingResponse))]
[JsonSerializable(typeof(EmbeddingResult))]
[JsonSerializable(typeof(Error))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(ModelList))]
[JsonSerializable(typeof(ResponseFormat))]
[JsonSerializable(typeof(Usage))]
public sealed partial class JsonContext : JsonSerializerContext;

#endif