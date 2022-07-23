using AspNetIntegrationTesting.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AspNetIntegrationTesting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;
        private readonly IPdfService _pdfService;
        private readonly ContentOptions _contentOptions;

        public ContentController(ILogger<ContentController> logger, IPdfService pdfService, IOptions<ContentOptions> contentOptions)
        {
            _logger = logger;
            _pdfService = pdfService;
            _contentOptions = contentOptions.Value;
        }

        [HttpGet]
        [Route("pdf/{sourceId}")]
        public async Task<FileResult> GetPdf(int? sourceId)
        {
            var url = _contentOptions[sourceId ?? 0];

            return File(await _pdfService.GetPdfFromUrl(url), "application/pdf");
        }
    }
}