using AspNetIntegrationTesting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ComponentTesting
{
    [UsesVerify]
    public class Samples
    {
        [Fact]
        public async Task VerifyPdf()
        {
            // Given the application
            await using var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.Configure<ContentOptions>((x) =>
                        {
                            x.Clear();
                            x.Add("data:text/html,%3Ch1%3EHello%2C%20World%21%3C%2Fh1%3E");
                        });
                    });
                });

            // When called
            using var httpClient = application.CreateClient();
            using var response = await httpClient.GetAsync($"/content/pdf/0");
            using var pdfStream = await response.Content.ReadAsStreamAsync();

            // Then should return the expected PDF
            await Verify(pdfStream)
                .UseExtension("pdf");
        }
    }
}