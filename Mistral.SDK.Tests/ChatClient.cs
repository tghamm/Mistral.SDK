using System.Text;
using System.Text.Json;
using Microsoft.Extensions.AI;
using Mistral.SDK.Converters;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class ChatClientTests
    {
        [TestMethod]
        public async Task TestMistralCompletionModel()
        {
            IChatClient client = new MistralClient().Completions;

            var response = await client.CompleteAsync(new List<ChatMessage>()
            {
                new(ChatRole.System, "You are an expert at writing sonnets."),
                new(ChatRole.User, "Write me a sonnet about the Statue of Liberty.")
            }, new() { ModelId = ModelDefinitions.OpenMistral7b }).ConfigureAwait(false);

            Assert.IsTrue(!string.IsNullOrEmpty(response.Message.Text));
        }

        public class JsonResult
        {
            public string result { get; set; }
        }

        [TestMethod]
        public async Task TestMistralCompletionJsonMode()
        {
            IChatClient client = new MistralClient().Completions;

            var response = await client.CompleteAsync(new List<ChatMessage>()
            {
                new(ChatRole.System, "You are an expert at writing Json."),
                new(ChatRole.User, "Write me a simple 'hello world' statement in a json object with a single 'result' key.")
            }, new() { ModelId = ModelDefinitions.MistralLarge, ResponseFormat = ChatResponseFormat.Json }).ConfigureAwait(false);

            Assert.IsTrue(!string.IsNullOrEmpty(response.Message.Text));

            //parse json
            Assert.IsNotNull(JsonSerializer.Deserialize<JsonResult>(response.Message.Text));
        }

        [TestMethod]
        public async Task TestMistralCompletionJsonModeStreaming()
        {
            IChatClient client = new MistralClient().Completions;

            var sb = new StringBuilder();
            await foreach (var update in client.CompleteStreamingAsync(new List<ChatMessage>()
                {
                    new(ChatRole.System, "You are an expert at writing Json."),
                    new(ChatRole.User, "Write me a simple 'hello world' statement in a json object with a single 'result' key.")
                }, new() { ModelId = ModelDefinitions.MistralLarge, ResponseFormat = ChatResponseFormat.Json }).ConfigureAwait(false))
            {
                sb.Append(update);
            }

            //parse json
            Assert.IsNotNull(JsonSerializer.Deserialize<JsonResult>(sb.ToString()));
        }

        [TestMethod]
        public async Task TestMistralCompletionSafeWithOptions()
        {
            IChatClient client = new MistralClient().Completions;

            var response = await client.CompleteAsync(new List<ChatMessage>()
            {
                new(ChatRole.System, "You are an expert at writing sonnets."),
                new(ChatRole.User, "Write me a sonnet about the Statue of Liberty.")
            }, new()
            {
                ModelId = ModelDefinitions.MistralMedium,
                Temperature = 0,
                MaxOutputTokens = 500,
                Seed = 32,
                AdditionalProperties = new AdditionalPropertiesDictionary
                {
                    ["SafePrompt"] = true,
                },
            }).ConfigureAwait(false);

            Assert.IsTrue(!string.IsNullOrEmpty(response.Message.Text));
        }

        [TestMethod]
        public async Task TestNonStreamingFunctionCalls()
        {
            IChatClient client = new MistralClient().Completions
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();

            ChatOptions options = new()
            {
                ModelId = ModelDefinitions.MistralSmall,
                MaxOutputTokens = 512,
                ToolMode = ChatToolMode.Auto,
                Tools = [AIFunctionFactory.Create((string personName) => personName switch {
                    "Alice" => "25",
                    _ => "40"
                }, "GetPersonAge", "Gets the age of the person whose name is specified.")]
            };

            var res = await client.CompleteAsync("How old is Alice?", options);

            Assert.IsTrue(
                res.Message.Text?.Contains("25") is true,
                res.Message.Text);
        }

        [TestMethod]
        public async Task TestStreamingFunctionCalls()
        {
            IChatClient client = new MistralClient().Completions
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();

            ChatOptions options = new()
            {
                ModelId = ModelDefinitions.MistralSmall,
                MaxOutputTokens = 512,
                ToolMode = ChatToolMode.Auto,
                Tools = [AIFunctionFactory.Create((string personName) => personName switch {
                    "Alice" => "25",
                    _ => "40"
                }, "GetPersonAge", "Gets the age of the person whose name is specified.")]
            };

            StringBuilder sb = new();
            await foreach (var update in client.CompleteStreamingAsync("How old is Alice?", options))
            {
                sb.Append(update);
            }

            Assert.IsTrue(
                sb.ToString().Contains("25") is true,
                sb.ToString());
        }
    }
}