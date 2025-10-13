using Mistral.SDK.DTOs;

namespace Mistral.SDK.Tests
{
    [TestClass]
    public class OCR
    {
        // An example image containing the text "Hello World! Sample Document"
        const string sampleImage = "data:image/png;base64,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";

        // An example PDF document containing the text "Hello World! Sample Document"
        const string sampleDocument = "data:application/pdf;base64,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";

        [TestMethod]
        public async Task TestMistalOCRModelWithImage()
        {
            var client = new MistralClient();
            var request = new OCRRequest {
                Model = ModelDefinitions.MistralOCR,
                Document = new Document {
                    Type = "image_url",
                    ImageUrl = sampleImage
                },
                IncludeImageBase64 = false
            };

            var response = await client.OCR.GetOCRAsync(request).ConfigureAwait(false);

            // The OCR result should contain the text in the image
            StringAssert.Contains(
                string.Join(" ", response.Pages.Select(p => p.Markdown)),
                "Hello World!"
            );
        }

        [TestMethod]
        public async Task TestMistalOCRModelWithPDF()
        {
            var client = new MistralClient();
            var request = new OCRRequest {
                Model = ModelDefinitions.MistralOCR,
                Document = new Document {
                    Type = "document_url",
                    DocumentUrl = sampleDocument
                },
                IncludeImageBase64 = false
            };

            var response = await client.OCR.GetOCRAsync(request).ConfigureAwait(false);

            // The OCR result should contain the text in the document
            StringAssert.Contains(
                string.Join(" ", response.Pages.Select(p => p.Markdown)),
                "Sample Document"
            );
        }
    }
}
