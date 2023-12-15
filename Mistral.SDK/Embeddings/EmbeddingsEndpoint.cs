using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Embeddings
{
    public class EmbeddingsEndpoint: EndpointBase
    {
        /// <summary>
        /// Constructor of the api endpoint.  Rather than instantiating this yourself, access it through an instance of <see cref="MistralClient"/> as <see cref="MistralClient.Embeddings"/>.
        /// </summary>
        /// <param name="client"></param>
        internal EmbeddingsEndpoint(MistralClient client) : base(client) { }

        protected override string Endpoint => "embeddings";

        /// <summary>
        /// Makes a POST call to the Embeddings API.
        /// </summary>
        public async Task<EmbeddingResponse> GetEmbeddingsAsync(EmbeddingRequest request)
        {
            var response = await HttpRequestRaw(Url, HttpMethod.Post, request);
            string resultAsString = await response.Content.ReadAsStringAsync();

            var res = await JsonSerializer.DeserializeAsync<EmbeddingResponse>(
                new MemoryStream(Encoding.UTF8.GetBytes(resultAsString)));

            return res;
        }
    }
}
