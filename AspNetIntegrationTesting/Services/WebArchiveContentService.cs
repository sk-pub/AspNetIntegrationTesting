namespace AspNetIntegrationTesting.Services
{
    public class WebArchiveContentService : IContentService
    {
        private readonly HttpClient _httpClient;
        private string? _cachedContent;

        public WebArchiveContentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetContent()
        {
            if (_cachedContent != null)
            {
                return _cachedContent;
            }

            _cachedContent = await _httpClient.GetStringAsync("https://web.archive.org/");

            return _cachedContent;
        }
    }
}
