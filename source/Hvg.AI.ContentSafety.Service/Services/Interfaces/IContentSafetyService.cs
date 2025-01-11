

namespace Hvg.AI.ContentSafety.Service.Services.Interfaces
{
    public interface IContentSafetyService
    {
        public Task<IEnumerable<AnalysisDto>> AnalyzeImage(IFormFile formFile);
        public Task<IEnumerable<AnalysisDto>> AnalyzeText(string text);
    }
}
