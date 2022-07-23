using AspNetIntegrationTesting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace IntegrationTesting
{
    public class Application
    {
        private readonly string _environment;

        public Application([NotNull] string environment)
        {
            _environment = environment;
        }

        public async Task<Stream> GetPdfStreamAsync(int sourceId = 0)
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment(_environment);
                });

            // When called
            var httpClient = application.CreateClient();
            var response = await httpClient.GetAsync($"/content/pdf/{sourceId}");

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
