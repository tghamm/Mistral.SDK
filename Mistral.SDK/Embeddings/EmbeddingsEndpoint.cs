using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Embeddings
{
    public class EmbeddingsEndpoint: EndpointBase, IEmbeddingGenerator<string, Embedding<float>>
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
        public async Task<EmbeddingResponse> GetEmbeddingsAsync(EmbeddingRequest request, CancellationToken cancellationToken = default)
        {
            var response = await HttpRequestRaw(Url, HttpMethod.Post, request, cancellationToken: cancellationToken).ConfigureAwait(false);
            
#if NET8_0_OR_GREATER
            string resultAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
            string resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif

            var res = await JsonSerializer.DeserializeAsync<EmbeddingResponse>(
                new MemoryStream(Encoding.UTF8.GetBytes(resultAsString)), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return res;
        }

        async Task<GeneratedEmbeddings<Embedding<float>>> IEmbeddingGenerator<string, Embedding<float>>.GenerateAsync(
            IEnumerable<string> values, EmbeddingGenerationOptions options, CancellationToken cancellationToken)
        {
            var request = new EmbeddingRequest(
                model: options?.ModelId,
                input: values.ToList(),
                encodingFormat: EmbeddingRequest.EncodingFormatEnum.Float);

            var response = await GetEmbeddingsAsync(request, cancellationToken).ConfigureAwait(false);

            var now = DateTime.UtcNow;
            var embeddings = new GeneratedEmbeddings<Embedding<float>>();
            foreach (var result in response.Data)
            {
                embeddings.Add(new Embedding<float>(result.Embedding.Select(d => (float)d).ToArray())
                {
                    ModelId = response.Model,
                    CreatedAt = now,
                });
            }

            if (response.Usage is { } usage)
            {
                embeddings.Usage = new UsageDetails()
                {
                    InputTokenCount = usage.PromptTokens,
                    OutputTokenCount = usage.CompletionTokens,
                    TotalTokenCount = usage.TotalTokens,
                };
            }

            return embeddings;
        }

        object IEmbeddingGenerator<string, Embedding<float>>.GetService(Type serviceType, object key) =>
            key is null && serviceType?.IsInstanceOfType(this) is true ? this : null;

        void IDisposable.Dispose() { }

        EmbeddingGeneratorMetadata IEmbeddingGenerator<string, Embedding<float>>.Metadata =>
            _metadata ??= new EmbeddingGeneratorMetadata(nameof(MistralClient), new Uri(Url));

        private EmbeddingGeneratorMetadata _metadata;
    }
}
