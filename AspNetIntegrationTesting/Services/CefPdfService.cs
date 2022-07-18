using CefSharp;
using CefSharp.OffScreen;
using CefSharp.Web;

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

            Cef.Initialize(settings);
        }

        public async Task<Stream> GetPdfFromHtml(string html)
        {
            var htmlString = new HtmlString(html);
            using var page = new ChromiumWebBrowser(htmlString);
            await page.WaitForNavigationAsync();

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
