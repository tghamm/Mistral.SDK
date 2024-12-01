﻿using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Converters;

public static class MistralSdkJsonOption
{

#if NET8_0_OR_GREATER
    public static readonly JsonSerializerOptions Options = null;
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
[JsonSerializable(typeof(Common.Function))]
[JsonSerializable(typeof(List<Common.Function>))]
[JsonSerializable(typeof(Common.Tool))]
[JsonSerializable(typeof(List<Common.Tool>))]
[JsonSerializable(typeof(List<ChatMessage>))]
[JsonSerializable(typeof(decimal?))]
[JsonSerializable(typeof(bool?))]
[JsonSerializable(typeof(ToolChoiceType))]
public sealed partial class JsonContext : JsonSerializerContext;

#endif