using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mistral.SDK.DTOs;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class Embeddings
    {
        [TestMethod]
        public async Task TestMistralEmbeddings()
        {
            var client = new MistralClient();
            var request = new EmbeddingRequest(
                ModelDefinitions.MistralEmbed, 
                new List<string>() { "Hello world" }, 
                EmbeddingRequest.EncodingFormatEnum.Float);
            var response = await client.Embeddings.GetEmbeddingsAsync(request).ConfigureAwait(false);
            Assert.IsNotNull(response);
        }
    }
}
