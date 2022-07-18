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

        public ContentController(ILogger<ContentController> logger, IContentService contentService)
        {
            _logger = logger;
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return await _contentService.GetContent();
        }
    }
}