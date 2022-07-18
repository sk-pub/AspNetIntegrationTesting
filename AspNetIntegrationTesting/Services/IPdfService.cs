namespace AspNetIntegrationTesting.Services
{
    public interface IPdfService
    {
        Task<Stream> GetPdfFromHtml(string html);
    }
}
