using AspNetIntegrationTesting.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetIntegrationTesting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;
        private readonly IPdfService _pdfService;

        public ContentController(ILogger<ContentController> logger, IPdfService pdfService)
        {
            _logger = logger;
            _pdfService = pdfService;
        }

        [HttpGet]
        [Route("pdf/{sourceId}")]
        public async Task<FileResult> GetPdf(int? sourceId)
        {
            var sources = new List<string>
            {
                "https://picsum.photos/",
                "https://web.archive.org/"
            };

            var url = sources[sourceId ?? 0];

            return File(await _pdfService.GetPdfFromUrl(url), "application/pdf");
        }
    }
}