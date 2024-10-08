﻿using Mistral.SDK.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mistral.SDK.DTOs
{
    public class ChatCompletionRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatCompletionRequest" /> class.
        /// </summary>
        /// <param name="model">ID of the model to use. You can use the [List Available Models](/api#operation/listModels) API to see all of your available models, or see our [Model overview](/models) for model descriptions.  (required).</param>
        /// <param name="messages">The prompt(s) to generate completions for, encoded as a list of dict with role and content. The first prompt role should be &#x60;user&#x60; or &#x60;system&#x60;.  (required).</param>
        /// <param name="temperature">What sampling temperature to use, between 0.0 and 1.0. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.  We generally recommend altering this or &#x60;top_p&#x60; but not both.  (default to 0.7M).</param>
        /// <param name="topP">Nucleus sampling, where the model considers the results of the tokens with &#x60;top_p&#x60; probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.  We generally recommend altering this or &#x60;temperature&#x60; but not both.  (default to 1M).</param>
        /// <param name="maxTokens">The maximum number of tokens to generate in the completion.  The token count of your prompt plus &#x60;max_tokens&#x60; cannot exceed the model&#39;s context length.  .</param>
        /// <param name="stream">Whether to stream back partial progress. If set, tokens will be sent as data-only server-sent events as they become available, with the stream terminated by a data: [DONE] message. Otherwise, the server will hold the request open until the timeout or until completion, with the response containing the full result as JSON.  (default to false).</param>
        /// <param name="safePrompt">Whether to inject a safety prompt before all conversations.  (default to false).</param>
        /// <param name="randomSeed">The seed to use for random sampling. If set, different calls will generate deterministic results. .</param>
        /// <param name="responseFormat">The response format needed If set, the API will be forced to return the data in this mode</param>
        public ChatCompletionRequest(string model = default(string), List<ChatMessage> messages = default(List<ChatMessage>), decimal? temperature = 0.7M, decimal? topP = 1M, int? maxTokens = default(int?), bool? stream = false, bool safePrompt = false, int? randomSeed = default(int?), ResponseFormat responseFormat = default)
        {
            // to ensure "model" is required (not null)
            if (model == null)
            {
                throw new ArgumentNullException("model is a required property for ChatCompletionRequest and cannot be null");
            }
            this.Model = model;
            // to ensure "messages" is required (not null)
            if (messages == null)
            {
                throw new ArgumentNullException("messages is a required property for ChatCompletionRequest and cannot be null");
            }
            this.Messages = messages;
            // use default value if no "temperature" provided
            this.Temperature = temperature ?? 0.7M;
            // use default value if no "topP" provided
            this.TopP = topP ?? 1M;
            this.MaxTokens = maxTokens;
            // use default value if no "stream" provided
            this.Stream = stream ?? false;
            this.SafePrompt = safePrompt;
            this.RandomSeed = randomSeed;
            this.TopK = 40;
            this.RepeatPenalty = 1.1M;
            this.MinP = .05M;
            
            this.ResponseFormat = responseFormat;
        }
        /// <summary>
        /// ID of the model to use. You can use the [List Available Models](/api#operation/listModels) API to see all of your available models, or see our [Model overview](/models) for model descriptions. 
        /// </summary>
        /// <value>ID of the model to use. You can use the [List Available Models](/api#operation/listModels) API to see all of your available models, or see our [Model overview](/models) for model descriptions. </value>
        /// <example>mistral-tiny</example>
        [JsonPropertyName("model")]
        public string Model { get; set; }

        /// <summary>
        /// The prompt(s) to generate completions for, encoded as a list of dict with role and content. The first prompt role should be &#x60;user&#x60; or &#x60;system&#x60;. 
        /// </summary>
        /// <value>The prompt(s) to generate completions for, encoded as a list of dict with role and content. The first prompt role should be &#x60;user&#x60; or &#x60;system&#x60;. </value>
        /// <example>[{&quot;role&quot;:&quot;user&quot;,&quot;content&quot;:&quot;What is the best French cheese?&quot;}]</example>
        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; }

        /// <summary>
        /// What sampling temperature to use, between 0.0 and 1.0. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.  We generally recommend altering this or &#x60;top_p&#x60; but not both. 
        /// </summary>
        /// <value>What sampling temperature to use, between 0.0 and 1.0. Higher values like 0.8 will make the output more random, while lower values like 0.2 will make it more focused and deterministic.  We generally recommend altering this or &#x60;top_p&#x60; but not both. </value>
        /// <example>0.7</example>
        [JsonPropertyName("temperature")]
        public decimal? Temperature { get; set; }

        /// <summary>
        /// Nucleus sampling, where the model considers the results of the tokens with &#x60;top_p&#x60; probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.  We generally recommend altering this or &#x60;temperature&#x60; but not both. 
        /// </summary>
        /// <value>Nucleus sampling, where the model considers the results of the tokens with &#x60;top_p&#x60; probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.  We generally recommend altering this or &#x60;temperature&#x60; but not both. </value>
        /// <example>1</example>
        [JsonPropertyName("top_p")]
        public decimal? TopP { get; set; }

        [JsonPropertyName("seed")]
        public int? Seed { get; set; }

        [JsonPropertyName("top_k")]
        public int? TopK { get; set; }

        [JsonPropertyName("min_p")]
        public decimal? MinP { get; set; }

        [JsonPropertyName("repeat_penalty")]
        public decimal? RepeatPenalty { get; set; }

        [JsonPropertyName("presence_penalty")]
        public decimal? PresencePenalty { get; set; }

        [JsonPropertyName("frequency_penalty")]
        public decimal? FrequencyPenalty { get; set; }

        [JsonPropertyName("logit_bias")]
        public decimal? LogitBias { get; set; }
        /*
        frequency_penalty,
        logit_bias,
        seed,
        */
        /// <summary>
        /// The maximum number of tokens to generate in the completion.  The token count of your prompt plus &#x60;max_tokens&#x60; cannot exceed the model&#39;s context length.  
        /// </summary>
        /// <value>The maximum number of tokens to generate in the completion.  The token count of your prompt plus &#x60;max_tokens&#x60; cannot exceed the model&#39;s context length.  </value>
        /// <example>16</example>
        [JsonPropertyName("max_tokens")]
        public int? MaxTokens { get; set; }

        /// <summary>
        /// Whether to stream back partial progress. If set, tokens will be sent as data-only server-sent events as they become available, with the stream terminated by a data: [DONE] message. Otherwise, the server will hold the request open until the timeout or until completion, with the response containing the full result as JSON. 
        /// </summary>
        /// <value>Whether to stream back partial progress. If set, tokens will be sent as data-only server-sent events as they become available, with the stream terminated by a data: [DONE] message. Otherwise, the server will hold the request open until the timeout or until completion, with the response containing the full result as JSON. </value>
        [JsonPropertyName("stream")]
        public bool? Stream { get; set; }

        /// <summary>
        /// Whether to inject a safety prompt before all conversations. 
        /// </summary>
        /// <value>Whether to inject a safety prompt before all conversations. </value>
        [JsonPropertyName("safe_prompt")]
        public bool SafePrompt { get; set; }

        /// <summary>
        /// The seed to use for random sampling. If set, different calls will generate deterministic results. 
        /// </summary>
        /// <value>The seed to use for random sampling. If set, different calls will generate deterministic results. </value>
        [JsonPropertyName("random_seed")]
        public int? RandomSeed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("response_format")]
        public ResponseFormat ResponseFormat { get; set; }  


		IEnumerable<ValidationResult> Validate()
        {
            // Temperature (decimal?) maximum
            if (this.Temperature > (decimal?)1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Temperature, must be a value less than or equal to 1.", new[] { "Temperature" });
            }

            // Temperature (decimal?) minimum
            if (this.Temperature < (decimal?)0)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for Temperature, must be a value greater than or equal to 0.", new[] { "Temperature" });
            }

            // TopP (decimal?) maximum
            if (this.TopP > (decimal?)1)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for TopP, must be a value less than or equal to 1.", new[] { "TopP" });
            }

            // TopP (decimal?) minimum
            if (this.TopP < (decimal?)0)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for TopP, must be a value greater than or equal to 0.", new[] { "TopP" });
            }

            // MaxTokens (int?) minimum
            if (this.MaxTokens < (int?)0)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("Invalid value for MaxTokens, must be a value greater than or equal to 0.", new[] { "MaxTokens" });
            }

            yield break;
        }
	}


}
