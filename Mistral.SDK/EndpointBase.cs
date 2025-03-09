﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Mistral.SDK.DTOs;
using System.Net;

namespace Mistral.SDK
{
    public abstract class EndpointBase
    {
        private const string UserAgent = "tghamm/mistral_sdk";

        /// <summary>
        /// The internal reference to the Client, mostly used for authentication
        /// </summary>
        protected readonly MistralClient Client;

        /// <summary>
        /// Constructor of the api endpoint base, to be called from the constructor of any derived classes.
        /// </summary>
        /// <param name="client"></param>
        internal EndpointBase(MistralClient client)
        {
            this.Client = client;
        }

        /// <summary>
        /// The name of the endpoint, which is the final path segment in the API URL.  Must be overriden in a derived class.
        /// </summary>
        protected abstract string Endpoint { get; }

        /// <summary>
        /// Gets the URL of the endpoint, based on the base OpenAI API URL followed by the endpoint name.  For example "https://api.mistral.ai/v1/chat/completions"
        /// </summary>
        protected string Url => string.Format(Client.ApiUrlFormat, Client.ApiVersion, Endpoint);

        internal static HttpClient client = null;

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

            var clientFactory = Client.HttpClientFactory;

            var client = clientFactory != null ? clientFactory.CreateClient() : new HttpClient(new HttpClientHandler
            {
                MaxConnectionsPerServer = 100,
                Proxy = new WebProxy(), // Add this line to disable proxy
                UseProxy = false, // Add this line to disable proxy
                SslProtocols = SslProtocols.Tls12, // Add this line to specify SSL protocols
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true // Add this line to bypass certificate validation
                
            });
            
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Client.Auth.ApiKey}");
            client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            return client;
        }

        private string GetErrorMessage(string resultAsString, HttpResponseMessage response, string name, string description = "")
        {
            return $"Error at {name} ({description}) with HTTP status code: {response.StatusCode}. Content: {resultAsString ?? "<no content>"}";
        }

        protected async Task<ChatCompletionResponse> HttpRequest(string url = null, HttpMethod verb = null, object postData = null)
        {
            var response = await HttpRequestRaw(url, verb, postData);
            string resultAsString = await response.Content.ReadAsStringAsync();

            var res = await JsonSerializer.DeserializeAsync<ChatCompletionResponse>(
                new MemoryStream(Encoding.UTF8.GetBytes(resultAsString)));

            return res;
        }

        protected async Task<HttpResponseMessage> HttpRequestRaw(string url = null, HttpMethod verb = null, object postData = null, bool streaming = false)
        {
            if (string.IsNullOrEmpty(url))
                url = this.Url;

            client = client ?? GetClient();

            client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            client.DefaultRequestHeaders.ConnectionClose = true;

            

            HttpResponseMessage response = null;
            string resultAsString = null;
            HttpRequestMessage req = new HttpRequestMessage(verb, url);
            req.Headers.ConnectionClose = true;
            

            if (postData != null)
            {
                if (postData is HttpContent)
                {
                    req.Content = postData as HttpContent;
                }
                else
                {
                    string jsonContent = JsonSerializer.Serialize(postData,
                        new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                    var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    req.Content = stringContent;
                }
            }

            response = await client.SendAsync(req,
                streaming ? HttpCompletionOption.ResponseHeadersRead : HttpCompletionOption.ResponseContentRead);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            else
            {
                try
                {
                    resultAsString = await response.Content.ReadAsStringAsync();
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

        protected async IAsyncEnumerable<ChatCompletionResponse> HttpStreamingRequest(string url = null, HttpMethod verb = null, object postData = null)
        {
            var response = await HttpRequestRaw(url, verb, postData, true);


            using var stream = await response.Content.ReadAsStreamAsync();
            using StreamReader reader = new StreamReader(stream);
            string line;
            SseEvent currentEvent = new SseEvent();
            while ((line = await reader.ReadLineAsync()) != null)
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
                            new MemoryStream(Encoding.UTF8.GetBytes(currentEvent.Data)));
                        yield return res;
                    }
                    else if (currentEvent.EventType != null)
                    {
                        var res = await JsonSerializer.DeserializeAsync<ErrorResponse>(
                            new MemoryStream(Encoding.UTF8.GetBytes(currentEvent.Data)));
                        throw new Exception(res.Error.Message);
                    }

                    // Reset the current event for the next one
                    currentEvent = new SseEvent();
                }
            }
        }
    }
}
