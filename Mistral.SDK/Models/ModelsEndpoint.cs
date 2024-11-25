using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Models
{
    public class ModelsEndpoint: EndpointBase
    {
        /// <summary>
        /// Constructor of the api endpoint.  Rather than instantiating this yourself, access it through an instance of <see cref="MistralClient"/> as <see cref="MistralClient.Models"/>.
        /// </summary>
        /// <param name="client"></param>
        internal ModelsEndpoint(MistralClient client) : base(client) { }

        protected override string Endpoint => "models";

        /// <summary>
        /// Makes a GET call to the Models API.
        /// </summary>
        public async Task<ModelList> GetModelsAsync(CancellationToken cancellationToken = default)
        {
            var response = await HttpRequestRaw(Url, HttpMethod.Get, cancellationToken: cancellationToken).ConfigureAwait(false);
            string resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var res = await JsonSerializer.DeserializeAsync<ModelList>(
                new MemoryStream(Encoding.UTF8.GetBytes(resultAsString)), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return res;
        }
    }
}
