namespace AspNetIntegrationTesting.Services
{
    public interface IPdfService
    {
        Task<Stream> GetPdfFromUrl(string url);
    }
}
