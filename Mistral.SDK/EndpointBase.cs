using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Mistral.SDK.Converters;
using Mistral.SDK.DTOs;

namespace Mistral.SDK
{
    public abstract class EndpointBase
    {
        private const string UserAgent = "tghamm/mistral_sdk";

        /// <summary>
        /// The internal reference to the Client, mostly used for authentication
        /// </summary>
        protected readonly MistralClient Client;

        private Lazy<HttpClient> _client;


        /// <summary>
        /// Constructor of the api endpoint base, to be called from the constructor of any derived classes.
        /// </summary>
        /// <param name="client"></param>
        internal EndpointBase(MistralClient client)
        {
            this.Client = client;
            _client = new Lazy<HttpClient>(GetClient);
        }
    
        /// <summary>
        /// The name of the endpoint, which is the final path segment in the API URL.  Must be overriden in a derived class.
        /// </summary>
        protected abstract string Endpoint { get; }

        /// <summary>
        /// Gets the URL of the endpoint, based on the base OpenAI API URL followed by the endpoint name.  For example "https://api.mistral.ai/v1/chat/completions"
        /// </summary>
        protected string Url => string.Format(Client.ApiUrlFormat, Client.ApiVersion, Endpoint);

        private HttpClient InnerClient => _client.Value;

        /// <summary>
        /// Gets an HTTPClient with the appropriate authorization and other headers set
        /// </summary>
        /// <returns>The fully initialized HttpClient</returns>
        /// <exception cref="AuthenticationException">Thrown if there is no valid authentication.</exception>
        protected HttpClient GetClient()
        {
            if (Client.Auth?.ApiKey is null)
            {
                throw new AuthenticationException("You must provide API authentication.");
            }

            var customClient = Client.HttpClient;

            var client = customClient ?? new HttpClient();

            if (!client.DefaultRequestHeaders.Contains("Authorization"))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Client.Auth.ApiKey}");
            }

            if (!client.DefaultRequestHeaders.Contains("User-Agent"))
            {
                client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            }
            
            return client;
        }

        private string GetErrorMessage(string resultAsString, HttpResponseMessage response, string name, string description = "")
        {
            return $"Error at {name} ({description}) with HTTP status code: {response.StatusCode}. Content: {resultAsString ?? "<no content>"}";
        }

        protected async Task<ChatCompletionResponse> HttpRequest(string url = null, HttpMethod verb = null, object postData = null, CancellationToken cancellationToken = default)
        {
            var response = await HttpRequestRaw(url, verb, postData, cancellationToken: cancellationToken).ConfigureAwait(false);
#if NET6_0_OR_GREATER
            string resultAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
            string resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif

            var res = await JsonSerializer.DeserializeAsync<ChatCompletionResponse>(
                new MemoryStream(Encoding.UTF8.GetBytes(resultAsString)),
                MistralClient.JsonSerializationOptions,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false); 

            return res;
        }

        protected async Task<HttpResponseMessage> HttpRequestRaw(string url = null, HttpMethod verb = null, object postData = null, bool streaming = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(url))
                url = this.Url;

            HttpResponseMessage response = null;
            string resultAsString = null;
            HttpRequestMessage req = new HttpRequestMessage(verb, url);

            if (postData != null)
            {
                if (postData is HttpContent)
                {
                    req.Content = postData as HttpContent;
                }
                else
                {
                    string jsonContent = JsonSerializer.Serialize(postData, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    req.Content = stringContent;
                }
            }
            // Ensure innerClient is thread-safe or use a separate instance per thread
            response = await InnerClient.SendAsync(req,
                streaming ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead, cancellationToken)
                .ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                try
                {
#if NET6_0_OR_GREATER
                    resultAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
                    resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
                }
                catch (Exception e)
                {
                    resultAsString =
                        "Additionally, the following error was thrown when attempting to read the response content: " +
                        e.ToString();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new AuthenticationException(
                        "Mistral rejected your authorization, most likely due to an invalid API Key. Full API response follows: " +
                        resultAsString);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException(
                        "Mistral had an internal server error, which can happen occasionally.  Please retry your request.  " +
                        GetErrorMessage(resultAsString, response, url, url));
                }
                else
                {
                    throw new HttpRequestException(GetErrorMessage(resultAsString, response, url, url));
                }
            }
        }

        protected async IAsyncEnumerable<ChatCompletionResponse> HttpStreamingRequest(string url = null, HttpMethod verb = null, object postData = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var response = await HttpRequestRaw(url, verb, postData, true, cancellationToken).ConfigureAwait(false);


#if NET6_0_OR_GREATER
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#else
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
            using StreamReader reader = new StreamReader(stream);
            string line;
            SseEvent currentEvent = new SseEvent();
            
#if NET8_0_OR_GREATER
            while ((line = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false)) != null)
#else
            while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) != null)
#endif
            {
                if (!string.IsNullOrEmpty(line))
                {
                    currentEvent.Data = line.Substring("data:".Length).Trim();
                }
                else // an empty line indicates the end of an event
                {
                    if (currentEvent.Data == "[DONE]")
                    {
                        continue;
                    }
                    else if (currentEvent.EventType == null)
                    {
                        var res = await JsonSerializer.DeserializeAsync<ChatCompletionResponse>(
                            new MemoryStream(Encoding.UTF8.GetBytes(currentEvent.Data)),
                            MistralClient.JsonSerializationOptions,
                            cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                        yield return res;
                    }
                    else if (currentEvent.EventType != null)
                    {
                        var res = await JsonSerializer.DeserializeAsync<ErrorResponse>(
                            new MemoryStream(Encoding.UTF8.GetBytes(currentEvent.Data)),
                            MistralClient.JsonSerializationOptions,
                            cancellationToken: cancellationToken)
                            .ConfigureAwait(false);
                        throw new Exception(res.Error.Message);
                    }

                    // Reset the current event for the next one
                    currentEvent = new SseEvent();
                }
            }
        }
    }
}
