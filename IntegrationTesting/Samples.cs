namespace IntegrationTesting
{
    [UsesVerify]
    public class Samples
    {
        private readonly Application _application = new Application();

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public Task VerifyPdf(int sourceId)
        {
            var parametersText = $"{sourceId}";

            return Verify(_application.GetPdfStreamAsync(sourceId))
                .UseTextForParameters(parametersText)
                .UseExtension("pdf")
                .UseMask($"{nameof(Samples)}/{nameof(Samples)}.{nameof(VerifyPdf)}_{parametersText}.*.mask.png");
        }
    }
}