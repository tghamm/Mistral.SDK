using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Tests;

[TestClass]
public class Parallel
{
    
    [TestMethod]
    public async Task TestParallelWithCustomHttpClient()
    {

        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(120) // Set timeout to 120 seconds
        };
        var client = new MistralClient(client: httpClient);
        var list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };

        await System.Threading.Tasks.Parallel.ForEachAsync(list, async (i, ctx) =>
        {
            var request = new ChatCompletionRequest(ModelDefinitions.OpenMistral7b, new List<ChatMessage>()
            {
                new ChatMessage(ChatMessage.RoleEnum.System, "You are an expert at writing sonnets."),
                new ChatMessage(ChatMessage.RoleEnum.User, "Write me a sonnet about the Statue of Liberty.")
            });
            var response = await client.Completions.GetCompletionAsync(request);
        });

    }
}