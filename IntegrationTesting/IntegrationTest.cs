using AspNetIntegrationTesting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace IntegrationTesting
{
    public class IntegrationTest
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async void ShouldReturnPdf(int id)
        {
            // Given the application
            await using var application = new WebApplicationFactory<Program>();

            // When called
            using var httpClient = application.CreateClient();
            using var response = await httpClient.GetAsync($"/content/pdf/{id}");

            // Then the app should respond with HTTP OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // And return a PDF file
            Assert.Equal("application/pdf", response.Content.Headers.ContentType.ToString());
        }
    }
}