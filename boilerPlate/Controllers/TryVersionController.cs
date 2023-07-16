using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using boilerPlate.Infra.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace boilerPlate.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class TryVersionController : Controller
    {
        private readonly ILogger<TryVersionController> _logger;
        IConfigService _config;
        public TryVersionController(ILogger<TryVersionController> logger, IConfigService config)
        {
            _logger = logger;
            _config = config;
        }
        [HttpGet]
        public IActionResult GetVersion()
        {
            string version = HttpContext.GetRequestedApiVersion().ToString();
            return Ok($"Version: {version}");
        }
        [HttpGet]
        public IActionResult GetVersionWithDt()
        {
            string version = HttpContext.GetRequestedApiVersion().ToString();
            return Ok($"Version: {version},Dt:{DateTime.Now.ToShortDateString()}");
        }

    }

    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public class TryVersionV2Controller : Controller
    {
        private readonly ILogger<TryVersionV2Controller> _logger;
        IConfigService _config;
        public TryVersionV2Controller(ILogger<TryVersionV2Controller> logger, IConfigService config)
        {
            _logger = logger;
            _config = config;
        }
        [HttpGet]
        public IActionResult GetVersion()
        {
            string version = HttpContext.GetRequestedApiVersion().ToString();
            return Ok($"Version: {version}");
        }
        [HttpGet]
        public IActionResult GetVersionWithDt()
        {
            string version = HttpContext.GetRequestedApiVersion().ToString();
            return Ok($"Version: {version},Dt:{DateTime.Now.ToLongDateString()}");
        }

    }
}

