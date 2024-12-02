using Mistral.SDK.Common;
using Mistral.SDK.DTOs;
using System.Globalization;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class FunctionCalling
    {
        public enum TempType
        {
            Fahrenheit,
            Celsius
        }

        [Function("This function returns the weather for a given location")]
        public static async Task<string> GetWeather([FunctionParameter("Location of the weather", true)] string location,
            [FunctionParameter("Unit of temperature, celsius or fahrenheit", true)] TempType tempType)
        {
            await Task.Yield();
            return "72 degrees and sunny";
        }

        [Function("Get the current user's name")]
        public static async Task<string> GetCurrentUser()
        {
            await Task.Yield();
            return "Mistral";
        }

        public static class StaticObjectTool
        {

            public static string GetWeather(string location)
            {
                return "72 degrees and sunny";
            }
        }

        public class InstanceObjectTool
        {

            public string GetWeather(string location)
            {
                return "72 degrees and sunny";
            }
        }

        [TestMethod]
        public async Task TestStaticObjectTool()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User, "What is the weather in San Francisco, CA?")
            };
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, messages);

            request.ToolChoice = ToolChoiceType.Auto;

            request.Tools = new List<Common.Tool>
            {
                Common.Tool.GetOrCreateTool(typeof(StaticObjectTool), nameof(GetWeather), "This function returns the weather for a given location")
            };

            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            messages.Add(response.Choices.First().Message);

            foreach (var toolCall in response.ToolCalls)
            {
                var resp = toolCall.Invoke<string>();
                messages.Add(new ChatMessage(toolCall, resp));
            }

            var finalResult = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            Assert.IsTrue(finalResult.Choices.First().Message.Content.Contains("72"));
        }

        [TestMethod]
        public async Task TestInstanceObjectTool()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User, "What is the weather in San Francisco, CA?")
            };
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, messages);

            request.ToolChoice = ToolChoiceType.Auto;

            var objectInstance = new InstanceObjectTool();
            request.Tools = new List<Common.Tool>
            {
                Common.Tool.GetOrCreateTool(objectInstance, nameof(GetWeather), "This function returns the weather for a given location")
            };

            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            messages.Add(response.Choices.First().Message);

            foreach (var toolCall in response.ToolCalls)
            {
                var resp = toolCall.Invoke<string>();
                messages.Add(new ChatMessage(toolCall, resp));
            }

            var finalResult = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            Assert.IsTrue(finalResult.Choices.First().Message.Content.Contains("72"));
        }

        [TestMethod]
        public async Task TestBasicFunctionStreaming()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User, "How many characters are in the word Christmas, multiply by 5, add 6, subtract 2, then divide by 2.1?")
            };
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, messages);

            request.ToolChoice = ToolChoiceType.Any;

            request.Tools = new List<Common.Tool>
            {
                Common.Tool.FromFunc("ChristmasMathFunction",
                    ([FunctionParameter("word to start with", true)]string word,
                        [FunctionParameter("number to multiply word count by", true)]int multiplier,
                        [FunctionParameter("amount to add to word count", true)]int addition,
                        [FunctionParameter("amount to subtract from word count", true)]int subtraction,
                        [FunctionParameter("amount to divide word count by", true)]double divisor) =>
                    {
                        return ((word.Length * multiplier + addition - subtraction) / divisor).ToString(CultureInfo.InvariantCulture);
                    }, "Function that can be used to determine the number of characters in a word combined with a mathematical formula")
            };
            var responses = new List<ChatCompletionResponse>();
            await foreach (var response in client.Completions.StreamCompletionAsync(request))
            {
                responses.Add(response);
            }

            messages.Add(responses.First(p => p.Choices.First().Delta.ToolCalls != null).Choices.First().Delta);

            foreach (var toolCall in responses.First(p => p.Choices.First().Delta.ToolCalls != null).ToolCalls)
            {
                var resp = toolCall.Invoke<string>();
                messages.Add(new ChatMessage(toolCall, resp));
            }

            request.ToolChoice = ToolChoiceType.none;

            var finalMessage = string.Empty;
            await foreach (var response in client.Completions.StreamCompletionAsync(request))
            {
                finalMessage += response.Choices.First().Delta.Content;
            }

            Assert.IsTrue(finalMessage.Contains("23"));
        }




        [TestMethod]
        public async Task TestBasicFunction()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User, "What is the current weather in San Francisco?")
            };
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, messages);

            request.ToolChoice = ToolChoiceType.Auto;

            var tools = new List<Common.Tool>
            {
                Common.Tool.FromFunc("Get_Weather",
                    ([FunctionParameter("Location of the weather", true)]string location)=> "72 degrees and sunny")
            };


            request.Tools = tools;

            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            messages.Add(response.Choices.First().Message);

            foreach (var toolCall in response.ToolCalls)
            {
                var resp = toolCall.Invoke<string>();
                messages.Add(new ChatMessage(toolCall, resp));
            }

            var finalResult = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            Assert.IsTrue(finalResult.Choices.First().Message.Content.Contains("72"));
        }

        [TestMethod]
        public async Task TestBasicToolDeclaredGlobally()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User, "What is the weather in San Francisco, CA in Fahrenheit?")
            };
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, messages);
            request.MaxTokens = 1024;
            request.Temperature = 0.0m;
            request.ToolChoice = ToolChoiceType.Auto;

            request.Tools = Common.Tool.GetAllAvailableTools(includeDefaults: false, forceUpdate: true, clearCache: true).ToList();

            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            messages.Add(response.Choices.First().Message);

            foreach (var toolCall in response.ToolCalls)
            {
                var resp = await toolCall.InvokeAsync<string>();
                messages.Add(new ChatMessage(toolCall, resp));
            }
            
            var finalResult = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            Assert.IsTrue(finalResult.Choices.First().Message.Content.Contains("72"));
        }

        [TestMethod]
        public async Task TestTestEmptyArgsAndMultiTool()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User, "What is the current user's name?")
            };
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, messages);

            request.ToolChoice = ToolChoiceType.Auto;

            request.Tools = Common.Tool.GetAllAvailableTools(includeDefaults: false, forceUpdate: true, clearCache: true).ToList();

            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            messages.Add(response.Choices.First().Message);

            foreach (var toolCall in response.ToolCalls)
            {
                var resp = await toolCall.InvokeAsync<string>();
                messages.Add(new ChatMessage(toolCall, resp));
            }

            var finalResult = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            Assert.IsTrue(finalResult.Choices.First().Message.Content.Contains("Mistral"));
        }

        [TestMethod]
        public async Task TestMathFuncTool()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User,"How many characters are in the word Christmas, multiply by 5, add 6, subtract 2, then divide by 2.1?")
            };
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, messages);

            request.ToolChoice = ToolChoiceType.Auto;

            request.Tools = new List<Common.Tool>
            {
                Common.Tool.FromFunc("ChristmasMathFunction",
                    ([FunctionParameter("word to start with", true)]string word,
                        [FunctionParameter("number to multiply word count by", true)]int multiplier,
                        [FunctionParameter("amount to add to word count", true)]int addition,
                        [FunctionParameter("amount to subtract from word count", true)]int subtraction,
                        [FunctionParameter("amount to divide word count by", true)]double divisor) =>
                    {
                        return ((word.Length * multiplier + addition - subtraction) / divisor).ToString(CultureInfo.InvariantCulture);
                    }, "Function that can be used to determine the number of characters in a word combined with a mathematical formula")
            };

            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            messages.Add(response.Choices.First().Message);

            foreach (var toolCall in response.ToolCalls)
            {
                var resp = toolCall.Invoke<string>();
                messages.Add(new ChatMessage(toolCall, resp));
            }

            var finalResult = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            Assert.IsTrue(finalResult.Choices.First().Message.Content.Contains("23"));
        }


        [TestMethod]
        public async Task TestBoolTool()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User,"Should I wear a hat? It's warm outside.")
            };
            var request = new ChatCompletionRequest(ModelDefinitions.MistralSmall, messages);

            request.ToolChoice = ToolChoiceType.Any;

            request.Tools = new List<Common.Tool>
            {
                Common.Tool.FromFunc("Hat_Determiner",
                    ([FunctionParameter("Is it cold outside", true)]bool isItCold)=>
                    {
                        return "no";
                    }, "Determines whether you should wear a heat based on whether it's cold outside.")
            };

            var response = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            messages.Add(response.Choices.First().Message);
            var hitTool = false;
            foreach (var toolCall in response.ToolCalls)
            {
                var resp = toolCall.Invoke<string>();
                messages.Add(new ChatMessage(toolCall, resp));
                hitTool = true;
            }
            request.ToolChoice = ToolChoiceType.none;
            var finalResult = await client.Completions.GetCompletionAsync(request).ConfigureAwait(false);

            Assert.IsTrue(hitTool);
        }


    }
}
