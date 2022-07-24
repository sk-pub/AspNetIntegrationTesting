using AspNetIntegrationTesting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Diagnostics.CodeAnalysis;

namespace IntegrationTesting
{
    public class Application : IAsyncDisposable
    {
        private readonly WebApplicationFactory<Program> _application;
        private readonly HttpClient _httpClient;

        public Application([NotNull] string environment)
        {
            _application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment(environment);
                });

            _httpClient = _application.CreateClient();
        }

        public async ValueTask DisposeAsync()
        {
            _httpClient.Dispose();

            await _application.DisposeAsync();
        }

        public async Task<Stream> GetPdfStreamAsync(int sourceId = 0)
        {
            var response = await _httpClient.GetAsync($"/content/pdf/{sourceId}");

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
