namespace IntegrationTesting
{
    [UsesVerify]
    public class TestingSamples
    {
        private readonly Application _application = new Application("Testing");

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