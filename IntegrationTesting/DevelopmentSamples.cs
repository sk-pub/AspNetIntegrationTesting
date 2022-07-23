namespace IntegrationTesting
{
    [UsesVerify]
    public class DevelopmentSamples
    {
        private readonly Application _application = new Application("Development");

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public Task VerifyPdf(int sourceId)
        {
            var parametersText = $"{sourceId}";

            return Verify(_application.GetPdfStreamAsync(sourceId))
                .UseTextForParameters(parametersText)
                .UseExtension("pdf")
                .UseMask($"{nameof(DevelopmentSamples)}/{nameof(DevelopmentSamples)}.{nameof(VerifyPdf)}_{parametersText}.*.mask.png");
        }
    }
}