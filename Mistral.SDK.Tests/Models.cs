using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class Models
    {
        [TestMethod]
        public async Task TestMistralModelList()
        {
            var client = new MistralClient();
            
            var response = await client.Models.GetModelsAsync().ConfigureAwait(false);
            Assert.IsNotNull(response);
        }
    }
}
