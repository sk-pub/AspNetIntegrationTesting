using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace AspNetIntegrationTesting.Services
{
    public sealed class PuppeteerPdfService : IPdfService, IAsyncDisposable
    {
        private readonly SemaphoreSlim _browserLock = new SemaphoreSlim(1, 1);
        private Browser? _browser;
        private readonly string _browserDownloadPath;

        private static bool IsRunningInContainer =>
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        public PuppeteerPdfService()
        {
            if (IsRunningInContainer)
            {
                return;
            }

            // Download Chrome always to the same folder
            _browserDownloadPath = Path.Combine(
                Path.GetTempPath(),
                "PuppeteerChrome");
        }

        public async ValueTask DisposeAsync()
        {
            await _browserLock.WaitAsync();

            try
            {
                if (_browser != null)
                {
                    await _browser.DisposeAsync();
                    _browser = null;
                }
            }
            finally
            {
                _browserLock.Release();
            }
        }

        public async Task<Stream> GetPdfFromUrl(string url)
        {
            await Initialize();

            await using var page = await _browser.NewPageAsync();
            await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);
            await page.EvaluateExpressionHandleAsync("document.fonts.ready");

            var pdfStream = await page.PdfStreamAsync(
                new PdfOptions
                {
                    Format = new PaperFormat(8.25m, 11.75m)
                });

            return pdfStream;
        }

        public async Task Initialize() // Should be initialized on the app start
        {
            await _browserLock.WaitAsync();

            try
            {
                if (_browser == null || _browser.IsClosed)
                {
                    if (_browser != null)
                    {
                        // Browser got closed somehow. E.G. when the Chrome process is killed
                        _browser.Dispose();
                        _browser = null;
                    }

                    var launchOptions = new LaunchOptions
                    {
                        Headless = true,
                        Args = new[]
                        {
                            "--disable-gpu",
                            "--disable-gpu-compositing",
                            "--enable-begin-frame-scheduling",
                            "--no-sandbox" // Required to run under root in a docker container
                        }
                    };

                    if (!IsRunningInContainer)
                    {
                        using var browserFetcher = new BrowserFetcher(
                            new BrowserFetcherOptions { Path = _browserDownloadPath });

                        await browserFetcher.DownloadAsync();

                        launchOptions.ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).ExecutablePath;
                    }

                    _browser = await Puppeteer.LaunchAsync(launchOptions);
                }
            }
            finally
            {
                _browserLock.Release();
            }
        }
    }
}