using CefSharp;
using CefSharp.OffScreen;

namespace AspNetIntegrationTesting.Services
{
    public sealed class CefPdfService : IPdfService
    {
        private static readonly string _cachePath;

        static CefPdfService()
        {
            _cachePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_cachePath);

            var settings = new CefSettings();
            settings.SetOffScreenRenderingBestPerformanceArgs();
            settings.DisableGpuAcceleration();
            settings.CachePath = _cachePath;
            settings.BackgroundColor = Cef.ColorSetARGB(1, 255, 255, 255);

            settings.RemoteDebuggingPort = 8088;

            Cef.Initialize(settings);
        }

        public async Task<Stream> GetPdfFromUrl(string url)
        {
            var browserSettings = new BrowserSettings { BackgroundColor = Cef.ColorSetARGB(1, 255, 255, 255) };
            using var page = new ChromiumWebBrowser(url, browserSettings);

            await page.WaitForNavigationAsync();
            //await Task.Delay(100000);

            var path = Path.GetTempFileName();

            var done = await page.PrintToPdfAsync(path);

            if (!done)
            {
                throw new Exception("Error while creating PDF");
            }

            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, FileOptions.DeleteOnClose | FileOptions.Asynchronous);
        }
    }
}
