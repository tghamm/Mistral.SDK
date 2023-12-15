using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Completions
{
    public class CompletionsEndpoint : EndpointBase
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

        
    }
}
