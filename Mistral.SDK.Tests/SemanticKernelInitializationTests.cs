﻿using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable SKEXP0001

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class SemanticKernelInitializationTests
    {
        [TestMethod]
        public async Task TestSKInit()
        {
            var skChatService =
                new ChatClientBuilder(new MistralClient().Completions)
                    .UseFunctionInvocation()
                    .Build()
                    .AsChatCompletionService();


            var sk = Kernel.CreateBuilder();
            sk.Plugins.AddFromType<SkPlugins>("Weather");
            sk.Services.AddSingleton<IChatCompletionService>(skChatService);

            var kernel = sk.Build();
            var chatCompletionService = kernel.Services.GetRequiredService<IChatCompletionService>();
            // Create chat history
            var history = new ChatHistory();
            history.AddUserMessage("What is the weather like in San Francisco right now?");
            OpenAIPromptExecutionSettings promptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                ModelId = ModelDefinitions.MistralLarge,
                MaxTokens = 1024,
                Temperature = 0.0,
            };

            // Get the response from the AI
            var result = await chatCompletionService.GetChatMessageContentAsync(
                history,
                executionSettings: promptExecutionSettings,
                kernel: kernel
            ); ;


            Assert.IsTrue(result.Content.Contains("72"));
        }
    }

    public class SkPlugins
    {
        [KernelFunction("GetWeather")]
        [Description("Gets the weather for a given location")]
        public async Task<string> GetWeather(string location)
        {
            return "It is 72 degrees and sunny in " + location;
        }
    }
}
