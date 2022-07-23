namespace IntegrationTesting
{
    [UsesVerify]
    public class Samples
    {
        private readonly Application _application = new Application();

        [Fact]
        public Task VerifyPdf()
        {
            return Verify(_application.GetPdfStreamAsync())
                .UseExtension("pdf")
                .UseMask($"{nameof(Samples)}.{nameof(VerifyPdf)}.*.mask.png");
        }
    }
}