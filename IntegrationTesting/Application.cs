using AspNetIntegrationTesting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTesting
{
    public class Application
    {
        public async Task<Stream> GetPdfStreamAsync()
        {
            var application = new WebApplicationFactory<Program>();

            // When called
            var httpClient = application.CreateClient();
            var response = await httpClient.GetAsync("/content/pdf");

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
