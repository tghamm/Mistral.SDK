using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class Completions
    {
        [TestMethod]
        public async Task TestMistralCompletion()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.MistralMedium, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            });
            var response = await client.Completions.GetCompletionAsync(request);
            
        }

        [TestMethod]
        public async Task TestMistralCompletionSafeWithOptions()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(
                //define model - required
                ModelDefinitions.MistralMedium,
                //define messages - required
                new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, 
                    "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, 
                    "Write me a sonnet about the Statue of Liberty.")
            }, 
                //optional - defaults to false
                safePrompt: true, 
                //optional - defaults to 0.7
                temperature: 0, 
                //optional - defaults to null
                maxTokens: 500, 
                //optional - defaults to 1
                topP: 1, 
                //optional - defaults to null
                randomSeed: 32);
            var response = await client.Completions.GetCompletionAsync(request);

        }

        [TestMethod]
        public async Task TestMistralCompletionStreaming()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(
                ModelDefinitions.MistralMedium, 
                new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, 
                    "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, 
                    "Write me a sonnet about the Statue of Liberty.")
            });
            var results = new List<ChatCompletionResponse>();
            await foreach (var res in client.Completions.StreamCompletionAsync(request))
            {
                results.Add(res);
                Debug.Write(res.Choices.First().Delta.Content);
            }
            Assert.IsTrue(results.Any());
        }

        [TestMethod]
        public async Task TestMistralCompletionStreamingSafePrompt()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.MistralMedium, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            }, safePrompt: true);
            var results = new List<ChatCompletionResponse>();
            await foreach (var res in client.Completions.StreamCompletionAsync(request))
            {
                results.Add(res);
                Debug.Write(res.Choices.First().Delta.Content);
            }
            Assert.IsTrue(results.Any());
        }
    }
}