using Microsoft.Extensions.AI;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class EmbeddingGeneratorTests
    {
        [TestMethod]
        public async Task TestMistralEmbeddingsGenerator()
        {
            IEmbeddingGenerator<string, Embedding<float>> client = new MistralClient().Embeddings;
            var response = await client.GenerateVectorAsync("hello world", new() { ModelId = ModelDefinitions.MistralEmbed }).ConfigureAwait(false);
            Assert.IsTrue(!response.IsEmpty);
        }
    }
}
