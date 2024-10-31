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
            var response = await client.GenerateEmbeddingVectorAsync("hello world", new() { ModelId = ModelDefinitions.MistralEmbed });
            Assert.IsTrue(!response.IsEmpty);
        }
    }
}
