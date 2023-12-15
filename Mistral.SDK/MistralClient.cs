using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Mistral.SDK.Completions;
using Mistral.SDK.Embeddings;
using Mistral.SDK.Models;

namespace Mistral.SDK
{
    public class MistralClient
    {
        public string ApiUrlFormat { get; set; } = "https://api.mistral.ai/{0}/{1}";

        /// <summary>
        /// Version of the Rest Api
        /// </summary>
        public string ApiVersion { get; set; } = "v1";

        /// <summary>
        /// The API authentication information to use for API calls
        /// </summary>
        public APIAuthentication Auth { get; set; }

        /// <summary>
        /// Optionally provide an IHttpClientFactory to create the client to send requests.
        /// </summary>
        public IHttpClientFactory HttpClientFactory { get; set; }

        public MistralClient(APIAuthentication apiKeys = null)
        {
            this.Auth = apiKeys.ThisOrDefault();
            Completions = new CompletionsEndpoint(this);
            Models = new ModelsEndpoint(this);
            Embeddings = new EmbeddingsEndpoint(this);
        }

        /// <summary>
        /// Text generation is the core function of the API. You give the API a prompt, and it generates a completion. The way you “program” the API to do a task is by simply describing the task in plain english or providing a few written examples. This simple approach works for a wide range of use cases, including summarization, translation, grammar correction, question answering, chatbots, composing emails, and much more (see the prompt library for inspiration).
        /// </summary>
        public CompletionsEndpoint Completions { get; }

        /// <summary>
        /// Lists the core models available to the user via API.
        /// </summary>
        public ModelsEndpoint Models { get; }
        
        /// <summary>
        /// Gets model embeddings via API.
        /// </summary>
        public EmbeddingsEndpoint Embeddings { get; }
    }
}
