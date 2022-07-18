namespace AspNetIntegrationTesting.Services
{
    public class DieZeitContentService : IContentService
    {
        private readonly HttpClient _httpClient;
        private string? _cachedContent;

        public DieZeitContentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetContent()
        {
            if (_cachedContent != null)
            {
                return _cachedContent;
            }

            _cachedContent = await _httpClient.GetStringAsync("https://www.zeit.de/wissen/index");

            return _cachedContent;
        }
    }
}
