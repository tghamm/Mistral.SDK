

using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.OCR
{
    public class OCREndpoint : EndpointBase
    {
        /// <summary>
        /// Constructor of the api endpoint.  Rather than instantiating this yourself, access it through an instance of <see cref="MistralClient"/> as <see cref="MistralClient.OCR"/>.
        /// </summary>
        /// <param name="client"></param>
        internal OCREndpoint(MistralClient client) : base(client) { }

        /// <inheritdoc/>
        protected override string Endpoint => "ocr";

        public async Task<OCRResponse> GetOCRAsync(OCRRequest request, CancellationToken cancellationToken = default) {
            var response = await HttpRequestRaw(Url, HttpMethod.Post, request, cancellationToken: cancellationToken).ConfigureAwait(false);

#if NET8_0_OR_GREATER
            string resultAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
            string resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif

            return await JsonSerializer.DeserializeAsync<OCRResponse>(
                new MemoryStream(Encoding.UTF8.GetBytes(resultAsString)), cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }


    }
}
