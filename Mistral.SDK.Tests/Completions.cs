using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mistral.SDK.Converters;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class Completions
    {
        [TestMethod]
        public async Task TestMistralCompletionModel1()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.OpenMistral7b, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            });
            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);
            
        }
        [TestMethod]
        public async Task TestMistralCompletionModel2()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.OpenMixtral8x7b, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            });
            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

        }
        [TestMethod]
        public async Task TestMistralCompletionModel3()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            });
            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

        }
        [TestMethod]
        public async Task TestMistralCompletionModel4()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.MistralMedium, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            });
            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

        }
        [TestMethod]
        public async Task TestMistralCompletionModel5()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.MistralLarge, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            });
            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

        }

        public class JsonResult
        {
            public string result { get; set; }
        }

        [TestMethod]
        public async Task TestMistralCompletionJsonMode()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.MistralLarge, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing Json."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a simple 'hello world' statement in a json object with a single 'result' key.")
            }, responseFormat: new ResponseFormat()
            {
                Type = ResponseFormat.ResponseFormatEnum.JSON
            });
            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);
            //parse json
            var result = JsonSerializer.Deserialize<JsonResult>(response.Choices.First().Message.Content);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestMistralCompletionJsonModeStreaming()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(ModelDefinitions.MistralLarge, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing Json."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a simple 'hello world' statement in a json object with a single 'result' key.")
            }, responseFormat: new ResponseFormat()
            {
                Type = ResponseFormat.ResponseFormatEnum.JSON
            });
            var response = string.Empty;
            await foreach (var res in client.Completions.StreamCompletionAsync(request).ConfigureAwait(false))
            {
                response += res.Choices.First().Delta.Content;
            }
            //parse json
            var result = JsonSerializer.Deserialize<JsonResult>(response);
            Assert.IsNotNull(result);
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
            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

        }

        [TestMethod]
        public async Task TestMistralCompletionStreaming()
        {
            var client = new MistralClient();
            var request = new ChatCompletionRequest(
                ModelDefinitions.MistralLarge, 
                new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, 
                    "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, 
                    "Write me a sonnet about the Statue of Liberty.")
            });
            var results = new List<ChatCompletionResponse>();
            await foreach (var res in client.Completions.StreamCompletionAsync(request).ConfigureAwait(false))
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
            await foreach (var res in client.Completions.StreamCompletionAsync(request).ConfigureAwait(false))
            {
                results.Add(res);
                Debug.Write(res.Choices.First().Delta.Content);
            }
            Assert.IsTrue(results.Any());
        }
    }
}