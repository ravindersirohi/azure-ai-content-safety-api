
namespace Hvg.AI.ContentSafety.Service.Services
{
    public class ContentSafetyService : IContentSafetyService
    {
        private readonly ILogger<ContentSafetyService> _logger;
        private ContentSafetyClient? client;
        public ContentSafetyService(ILogger<ContentSafetyService> logger)
        {
            _logger = logger;
            SetClient();
        }

        public async Task<IEnumerable<AnalysisDto>> AnalyzeText(string text)
        {
            try
            {
                if (client == null)
                    throw new NullReferenceException($"{nameof(client)} can not be null.");

                var response = await client.AnalyzeTextAsync(text);

                var result = response.Value.CategoriesAnalysis.Select(MapCatagoryResult).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in AnalyzeText {Message}", ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<AnalysisDto>> AnalyzeImage(IFormFile formFile)
        {
            try
            {
                if (client == null)
                    throw new NullReferenceException($"{nameof(client)} can not be null.");

                var imageSource = BinaryData.FromBytes(GetBase64Source(formFile));
                var response = await client.AnalyzeImageAsync(imageSource);

                var result = response.Value.CategoriesAnalysis.Select(MapCatagoryResult).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in AnalyzeImage {Message}", ex.Message);
                throw;
            }
        }

        private void SetClient()
        {
            var key = Environment.GetEnvironmentVariable("ContentSafetyApiKey");
            var endpoint = Environment.GetEnvironmentVariable("ContentSafetyApiEndpoint");
            if (key != null && endpoint != null)
            {
                client = new ContentSafetyClient(new Uri(endpoint), new AzureKeyCredential(key));
            }
        }

        private static byte[] GetBase64Source(IFormFile file)
        {
            var stream = file.OpenReadStream();
            byte[] bytes = [];
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        private static AnalysisDto MapCatagoryResult(TextCategoriesAnalysis imageCategoriesAnalyse)
        {
            AnalysisDto analysisDto = new() { Severity = imageCategoriesAnalyse.Severity };

            if (imageCategoriesAnalyse.Category == TextCategory.Hate)
                analysisDto.Category = ImageCategory.Hate.ToString();
            else if (imageCategoriesAnalyse.Category == TextCategory.SelfHarm)
                analysisDto.Category = ImageCategory.SelfHarm.ToString();
            else if (imageCategoriesAnalyse.Category == TextCategory.Sexual)
                analysisDto.Category = ImageCategory.Sexual.ToString();
            else if (imageCategoriesAnalyse.Category == TextCategory.Violence)
                analysisDto.Category = ImageCategory.Violence.ToString();

            return analysisDto;
        }

        private static AnalysisDto MapCatagoryResult(ImageCategoriesAnalysis imageCategoriesAnalyse)
        {
            AnalysisDto analysisDto = new() { Severity = imageCategoriesAnalyse.Severity };

            if (imageCategoriesAnalyse.Category == ImageCategory.Hate)
                analysisDto.Category = ImageCategory.Hate.ToString();
            else if (imageCategoriesAnalyse.Category == ImageCategory.SelfHarm)
                analysisDto.Category = ImageCategory.SelfHarm.ToString();
            else if (imageCategoriesAnalyse.Category == ImageCategory.Sexual)
                analysisDto.Category = ImageCategory.Sexual.ToString();
            else if (imageCategoriesAnalyse.Category == ImageCategory.Violence)
                analysisDto.Category = ImageCategory.Violence.ToString();

            return analysisDto;
        }
    }
}
