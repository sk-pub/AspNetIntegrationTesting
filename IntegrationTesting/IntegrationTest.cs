using AspNetIntegrationTesting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace IntegrationTesting
{
    public class IntegrationTest
    {
        [Fact]
        public async void ShouldReturnPdf()
        {
            // Given the application
            var application = new WebApplicationFactory<Program>();

            // When called
            var httpClient = application.CreateClient();
            var response = await httpClient.GetAsync("/content/pdf");

            // Then the app should respond with HTTP OK
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // And return a PDF file
            Assert.Equal("application/pdf", response.Content.Headers.ContentType.ToString());
        }
    }
}