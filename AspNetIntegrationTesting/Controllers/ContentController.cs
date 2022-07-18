using AspNetIntegrationTesting.Services;
using Microsoft.AspNetCore.Mvc;

namespace AspNetIntegrationTesting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> _logger;
        private readonly IContentService _contentService;
        private readonly IPdfService _pdfService;

        public ContentController(ILogger<ContentController> logger, IContentService contentService, IPdfService pdfService)
        {
            _logger = logger;
            _contentService = contentService;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return await _contentService.GetContent();
        }

        [HttpGet]
        [Route("pdf")]
        public async Task<FileResult> GetPdf()
        {
            var content = await _contentService.GetContent();

            return File(await _pdfService.GetPdfFromHtml(content), "application/pdf");
        }
    }
}