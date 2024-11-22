using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Completions
{
    public class CompletionsEndpoint : EndpointBase, IChatClient
    {
        /// <summary>
        /// Constructor of the api endpoint.  Rather than instantiating this yourself, access it through an instance of <see cref="MistralClient"/> as <see cref="MistralClient.Completions"/>.
        /// </summary>
        /// <param name="client"></param>
        internal CompletionsEndpoint(MistralClient client) : base(client) { }

        protected override string Endpoint => "chat/completions";

        /// <summary>
        /// Makes a non-streaming call to the completion API. Be sure to set stream to false in <param name="parameters"></param>.
        /// </summary>
        /// <param name="request"></param>
        public async Task<ChatCompletionResponse> GetCompletionAsync(ChatCompletionRequest request)
        {
            request.Stream = false;
            var response = await HttpRequest(Url, HttpMethod.Post, request);
            return response;
        }

        /// <summary>
        /// Makes a streaming call to the completion API using an IAsyncEnumerable. Be sure to set stream to true in <param name="request"></param>.
        /// </summary>
        /// <param name="request"></param>
        public async IAsyncEnumerable<ChatCompletionResponse> StreamCompletionAsync(ChatCompletionRequest request)
        {
            request.Stream = true;
            await foreach (var result in HttpStreamingRequest(Url, HttpMethod.Post, request))
            {
                yield return result;
            }
        }

        async Task<ChatCompletion> IChatClient.CompleteAsync(
            IList<Microsoft.Extensions.AI.ChatMessage> chatMessages, ChatOptions options, CancellationToken cancellationToken)
        {
            var response = await GetCompletionAsync(CreateRequest(chatMessages, options));

            var completion = new ChatCompletion(new List<Microsoft.Extensions.AI.ChatMessage>())
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

            foreach (Choice choice in response.Choices)
            {
                ChatRole role = choice.Message.Role switch
                {
                    DTOs.ChatMessage.RoleEnum.System => ChatRole.System,
                    DTOs.ChatMessage.RoleEnum.Assistant => ChatRole.User,
                    _ => ChatRole.User,
                };

                completion.Choices.Add(new Microsoft.Extensions.AI.ChatMessage(role, choice.Message.Content));

                if (completion.FinishReason is null && choice.FinishReason != null)
                {
                    completion.FinishReason = choice.FinishReason switch
                    {
                        Choice.FinishReasonEnum.Length => ChatFinishReason.Length,
                        Choice.FinishReasonEnum.ModelLength => ChatFinishReason.Length,
                        _ => ChatFinishReason.Stop
                    };
                }
            }

            return completion;
        }

        async IAsyncEnumerable<StreamingChatCompletionUpdate> IChatClient.CompleteStreamingAsync(
            IList<Microsoft.Extensions.AI.ChatMessage> chatMessages, ChatOptions options, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var response in StreamCompletionAsync(CreateRequest(chatMessages, options)))
            {
                foreach (var choice in response.Choices)
                {
                    yield return new StreamingChatCompletionUpdate
                    {
                        ChoiceIndex = choice.Index,
                        CompletionId = response.Id,
                        ModelId = response.Model,
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
                    DTOs.ChatMessage.RoleEnum.Assistant;

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

            return request;
        }

        void IDisposable.Dispose() { }

        object IChatClient.GetService(Type serviceType, object key) =>
            key is null && serviceType?.IsInstanceOfType(this) is true ? this : null;

        ChatClientMetadata IChatClient.Metadata => _metadata ??= new ChatClientMetadata(nameof(MistralClient), new Uri(Url));

        private ChatClientMetadata _metadata;
    }
}
