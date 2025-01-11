
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Hvg.AI.ContentSafety.Service
{
    public class ContentServiceApi
    {
        private readonly ILogger<ContentServiceApi> _logger;
        private readonly IContentSafetyService _contentSafetyService;
        public ContentServiceApi(ILogger<ContentServiceApi> logger, IContentSafetyService contentSafetyService)
        {
            _logger = logger;
            _contentSafetyService = contentSafetyService ?? throw new ArgumentNullException(nameof(contentSafetyService));
        }

        [Function("HealthCheck")]
        public IActionResult RunHealthCheck([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("HealthCheck trigger function processed a request.");
            return new OkObjectResult("Welcome to HealthCheck Functions!");
        }

        [Function("AnalyzeText")]
        public async Task<IActionResult> RunAnalyzeText([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
        {
            try
            {
                _logger.LogInformation("Analyze text request.");

                if (request.ContentLength == 0 || !request.ContentLength.HasValue)
                    return new BadRequestObjectResult("Empty or null input text.");
                else
                {
                    var content = await new StreamReader(request.Body).ReadToEndAsync();
                    var response = await _contentSafetyService.AnalyzeText(content);
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in AnalyzeImage {Message}", ex.Message);
                throw;
            }
        }

        [Function("AnalyzeImage")]
        public async Task<IActionResult> RunAnalyzeImage([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
        {
            try
            {
                _logger.LogInformation("Analyze Image request.");
                var formFile = request?.Form?.Files?.Count == 1 ? request.Form.Files[0] : throw new ArgumentException($"'{request?.Form?.Files?.Count}' file(s) found in form-data");

                if (!IsValidFileType(formFile.FileName.ToLower()))
                    return new BadRequestObjectResult("Invalid image file type supplied - jpg, jpeg, bmp, png and tiff supported.");
                else
                {
                    var response = await _contentSafetyService.AnalyzeImage(formFile);
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in AnalyzeImage {Message}", ex.Message);
                throw;
            }
        }

        private static bool IsValidFileType(string fileName)
        {
            return fileName.EndsWith(".jpg")
                || fileName.EndsWith(".jpeg")
                || fileName.EndsWith(".bmp")
                || fileName.EndsWith(".png")
                || fileName.EndsWith(".tiff");
        }
    }
}
