using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.AI;
using Mistral.SDK.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Mistral.SDK.Completions
{
    public partial class CompletionsEndpoint : IChatClient
    {

        async Task<ChatCompletion> IChatClient.CompleteAsync(
            IList<Microsoft.Extensions.AI.ChatMessage> chatMessages, ChatOptions options, CancellationToken cancellationToken)
        {
            var response = await GetCompletionAsync(CreateRequest(chatMessages, options), cancellationToken).ConfigureAwait(false);

            Microsoft.Extensions.AI.ChatMessage message = new(ChatRole.Assistant, ProcessResponseContent(response));

            var completion = new ChatCompletion(message)
            {
                CompletionId = response.Id,
                ModelId = response.Model
            };

            if (response.Usage is { } usage)
            {
                completion.Usage = new UsageDetails()
                {
                    InputTokenCount = usage.PromptTokens,
                    OutputTokenCount = usage.CompletionTokens,
                    TotalTokenCount = usage.TotalTokens
                };
            }

            return completion;
        }

        async IAsyncEnumerable<StreamingChatCompletionUpdate> IChatClient.CompleteStreamingAsync(
            IList<Microsoft.Extensions.AI.ChatMessage> chatMessages, ChatOptions options, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var response in StreamCompletionAsync(CreateRequest(chatMessages, options), cancellationToken).WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                foreach (var choice in response.Choices)
                {
                    var update = new StreamingChatCompletionUpdate {
                        ChoiceIndex = choice.Index,
                        CompletionId = response.Id,
                        ModelId = response.Model,
                        RawRepresentation = response,
                        Role = choice.Delta?.Role switch
                        {
                            DTOs.ChatMessage.RoleEnum.System => ChatRole.System,
                            DTOs.ChatMessage.RoleEnum.Assistant => ChatRole.User,
                            _ => ChatRole.User,
                        },
                        FinishReason = choice.FinishReason switch
                        {
                            Choice.FinishReasonEnum.Length => ChatFinishReason.Length,
                            Choice.FinishReasonEnum.ModelLength => ChatFinishReason.Length,
                            _ => ChatFinishReason.Stop
                        },
                        Text = choice.Delta?.Content,
                        
                    };

                    if (choice.Delta?.ToolCalls is { Count: > 0 })
                    {
                        update.Contents = new List<AIContent>
                        {
                            new TextContent(choice.Delta.Content)
                        };

                        foreach (var toolCall in choice.Delta.ToolCalls)
                        {
                            Dictionary<string, object> arguments = null;
                            if (toolCall.Function.Arguments is not null)
                            {
                                arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(toolCall.Function.Arguments.ToString());
                            }

                            update.Contents.Add(new FunctionCallContent(
                                toolCall.Id,
                                toolCall.Function.Name,
                                arguments));
                        }
                    }

                    yield return update;
                }

                if (response.Usage is { } usage)
                {
                    yield return new StreamingChatCompletionUpdate()
                    {
                        CompletionId = response.Id,
                        ModelId = response.Model,
                        Contents = new List<AIContent>()
                        {
                            new UsageContent(new UsageDetails()
                            {
                                InputTokenCount = usage.PromptTokens,
                                OutputTokenCount = usage.CompletionTokens,
                                TotalTokenCount = usage.TotalTokens
                            })
                        },
                    };
                }
            }
        }

        private static ChatCompletionRequest CreateRequest(IList<Microsoft.Extensions.AI.ChatMessage> chatMessages, ChatOptions options)
        {
            var messages = chatMessages.Select(m =>
            {
                DTOs.ChatMessage.RoleEnum role =
                    m.Role == ChatRole.System ? DTOs.ChatMessage.RoleEnum.System :
                    m.Role == ChatRole.User ? DTOs.ChatMessage.RoleEnum.User :
                    m.Role == ChatRole.Tool ? DTOs.ChatMessage.RoleEnum.Tool :
                    DTOs.ChatMessage.RoleEnum.Assistant;

                foreach (AIContent content in m.Contents)
                {
                    switch (content)
                    {
                        case Microsoft.Extensions.AI.FunctionResultContent frc:
                            return new DTOs.ChatMessage(frc.CallId, frc.Name, frc.Result?.ToString());
                        case Microsoft.Extensions.AI.FunctionCallContent fcc:
                            return new DTOs.ChatMessage()
                            {
                                Role = DTOs.ChatMessage.RoleEnum.Assistant,
                                ToolCalls = new List<ToolCall>()
                                {
                                    new ToolCall()
                                    {
                                        Id = fcc.CallId,
                                        Function = new ToolCallParameter()
                                        {
                                            Arguments = JsonSerializer.SerializeToNode(fcc.Arguments),
                                            Name = fcc.Name,
                                        }
                                    }
                                }
                            };
                    }
                }

                return new DTOs.ChatMessage(role, string.Concat(m.Contents.OfType<TextContent>()));
            }).ToList();

            var request = new ChatCompletionRequest(
                model: options?.ModelId,
                messages: messages,
                temperature: (decimal?)options?.Temperature,
                topP: (decimal?)options?.TopP,
                maxTokens: options?.MaxOutputTokens,
                safePrompt: options?.AdditionalProperties?.TryGetValue(nameof(ChatCompletionRequest.SafePrompt), out bool safePrompt) is true,
                randomSeed: (int?)options?.Seed);

            if (options.ResponseFormat is ChatResponseFormatJson)
            {
                request.ResponseFormat = new ResponseFormat() { Type = ResponseFormat.ResponseFormatEnum.JSON };
            }

            if (options.Tools is { Count: > 0 })
            {
                
                if (options.ToolMode is RequiredChatToolMode r)
                {
                    request.ToolChoice = ToolChoiceType.Any;
                }
                else if (options.ToolMode is AutoChatToolMode a)
                {
                    request.ToolChoice = ToolChoiceType.Auto;
                }

                request.Tools = options
                    .Tools
                    .OfType<AIFunction>()
                    .Select(f => new Common.Tool(new Common.Function(f.Metadata.Name, f.Metadata.Description, FunctionParameters.CreateSchema(f))))
                    .ToList();
            }

            return request;
        }

        private static List<AIContent> ProcessResponseContent(ChatCompletionResponse response)
        {
            List<AIContent> contents = new();

            foreach (var content in response.Choices)
            {
                if (content.Message.ToolCalls is not null)
                {
                    contents.Add(new Microsoft.Extensions.AI.TextContent(content.Message.Content));

                    foreach (var toolCall in content.Message.ToolCalls)
                    {
                        Dictionary<string, object> arguments = null;
                        if (toolCall.Function.Arguments is not null)
                        {
                            arguments = JsonSerializer.Deserialize<Dictionary<string, object>>(toolCall.Function.Arguments.ToString());
                        }

                        contents.Add(new FunctionCallContent(
                            toolCall.Id,
                            toolCall.Function.Name,
                            arguments));
                    }
                }
                else
                {
                    contents.Add(new Microsoft.Extensions.AI.TextContent(content.Message.Content));
                }
            }

            return contents;
        }

        void IDisposable.Dispose() { }

        object IChatClient.GetService(Type serviceType, object key) =>
            key is null && serviceType?.IsInstanceOfType(this) is true ? this : null;

        ChatClientMetadata IChatClient.Metadata => _metadata ??= new ChatClientMetadata(nameof(MistralClient), new Uri(Url));

        private ChatClientMetadata _metadata;


        private sealed class FunctionParameters
        {
            private static readonly JsonElement s_defaultParameterSchema = JsonDocument.Parse("{}").RootElement;

            [JsonPropertyName("type")]
            public string Type { get; set; } = "object";

            [JsonPropertyName("required")]
            public List<string> Required { get; set; } = [];

            [JsonPropertyName("properties")]
            public Dictionary<string, JsonElement> Properties { get; set; } = [];

            public static JsonNode CreateSchema(AIFunction f)
            {
                var parameters = f.Metadata.Parameters;

                FunctionParameters schema = new();

                foreach (AIFunctionParameterMetadata parameter in parameters)
                {
                    schema.Properties.Add(parameter.Name, parameter.Schema is JsonElement e ? e : s_defaultParameterSchema);

                    if (parameter.IsRequired)
                    {
                        schema.Required.Add(parameter.Name);
                    }
                }

                return JsonSerializer.SerializeToNode(schema);
            }
        }
    }

    
}
