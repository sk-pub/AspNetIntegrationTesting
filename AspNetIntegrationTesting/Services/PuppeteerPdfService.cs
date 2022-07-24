using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace AspNetIntegrationTesting.Services
{
    public sealed class PuppeteerPdfService : IPdfService, IAsyncDisposable
    {
        private readonly SemaphoreSlim _browserLock = new SemaphoreSlim(1, 1);
        private readonly string _userDataDir;
        private Browser? _browser;
        private readonly string _browserDownloadPath;

        public PuppeteerPdfService()
        {
            _userDataDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_userDataDir);

            _browserDownloadPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_browserDownloadPath);
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

                ClearDirectories();
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
                        ClearDirectories();
                    }

                    using var browserFetcher = new BrowserFetcher(
                        new BrowserFetcherOptions { Path = _browserDownloadPath });

                    await browserFetcher.DownloadAsync();

                    var launchOptions = new LaunchOptions
                    {
                        Headless = true,
                        UserDataDir = _userDataDir,
                        ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).ExecutablePath,
                        Args = new[]
                        {
                            "--disable-gpu",
                            "--disable-gpu-compositing",
                            "--enable-begin-frame-scheduling"
                        }
                    };
                    _browser = await Puppeteer.LaunchAsync(launchOptions);
                }
            }
            finally
            {
                _browserLock.Release();
            }
        }

        private void ClearDirectories()
        {
            if (Directory.Exists(_browserDownloadPath))
            {
                Directory.Delete(_browserDownloadPath, true);
            }

            if (Directory.Exists(_userDataDir))
            {
                Directory.Delete(_userDataDir, true);
            }
        }
    }
}