namespace AspNetIntegrationTesting.Services
{
    public class DieZeitContentService : IContentService
    {
        private readonly HttpClient _httpClient;

        public DieZeitContentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<string> GetContent()
        {
            return _httpClient.GetStringAsync("https://www.zeit.de/wissen/index");
        }
    }
}
