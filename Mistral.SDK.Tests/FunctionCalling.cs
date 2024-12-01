using Mistral.SDK.Common;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class FunctionCalling
    {
        [TestMethod]
        public async Task TestBasicFunction()
        {
            var client = new MistralClient();
            var messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.User, "What is the current weather in San Francisco?")
            };
            var request = new ChatCompletionRequest("mistral-large-latest", messages);

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
    }
}
