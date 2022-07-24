using AspNetIntegrationTesting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace ComponentTesting
{
    public class ComponentTest
    {
        [Fact]
        public async void ShouldReturnPdf()
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

            // Then the app should respond with HTTP OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // And return a PDF file
            Assert.Equal("application/pdf", response.Content.Headers.ContentType.ToString());
        }
    }
}