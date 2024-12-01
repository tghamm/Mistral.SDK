using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mistral.SDK.DTOs;
using ChatMessage = Mistral.SDK.DTOs.ChatMessage;

namespace Mistral.SDK.Completions
{
    public partial class CompletionsEndpoint : EndpointBase
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
        /// <param name="cancellationToken"></param>
        public async Task<ChatCompletionResponse> GetCompletionAsync(ChatCompletionRequest request, CancellationToken cancellationToken = default)
        {
            request.Stream = false;

            var response = await HttpRequest(Url, HttpMethod.Post, request, cancellationToken).ConfigureAwait(false);

            var toolCalls = new List<Common.Function>();
            foreach (var message in response.Choices)
            {
                if (message.Message.ToolCalls is null) continue;
                foreach (var returned_tool in message.Message.ToolCalls)
                {
                    var tool = request.Tools?.FirstOrDefault(t => t.Function.Name == returned_tool.Function.Name);
                    if (tool != null)
                    {
                        tool.Function.Arguments = returned_tool.Function.Arguments;
                        tool.Function.Id = returned_tool.Id;
                        toolCalls.Add(tool.Function);
                    }
                }
            }
            response.ToolCalls = toolCalls;



            return response;
        }

        /// <summary>
        /// Makes a streaming call to the completion API using an IAsyncEnumerable. Be sure to set stream to true in <param name="request"></param>.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        public async IAsyncEnumerable<ChatCompletionResponse> StreamCompletionAsync(ChatCompletionRequest request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            request.Stream = true;
            await foreach (var result in HttpStreamingRequest(Url, HttpMethod.Post, request, cancellationToken).WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                var toolCalls = new List<Common.Function>();
                foreach (var message in result.Choices)
                {
                    if (message.Delta.ToolCalls is null) continue;
                    foreach (var returned_tool in message.Delta.ToolCalls)
                    {
                        var tool = request.Tools?.FirstOrDefault(t => t.Function.Name == returned_tool.Function.Name);
                        if (tool != null)
                        {
                            tool.Function.Arguments = returned_tool.Function.Arguments;
                            tool.Function.Id = returned_tool.Id;
                            toolCalls.Add(tool.Function);
                        }
                    }
                }

                result.Choices.First().Delta.Role = ChatMessage.RoleEnum.Assistant;
                result.ToolCalls = toolCalls;
                yield return result;
            }
        }

        

        
    }
}
