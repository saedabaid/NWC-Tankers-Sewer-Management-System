using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NWC.Service.SignalR.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestsController : ControllerBase
    {

        private readonly ILogger<TestsController> _logger;

        public TestsController(ILogger<TestsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Works() => "Service works.";
    }
}
