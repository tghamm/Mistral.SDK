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
        public async Task TestMistralCompletionStreaming()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.MistralMedium, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            });
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