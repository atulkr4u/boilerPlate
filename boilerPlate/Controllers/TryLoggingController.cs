using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using boilerPlate.Infra.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace boilerPlate.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TryLoggingController : Controller
    {
        private readonly ILogger<TryLoggingController> _logger;
        public TryLoggingController(ILogger<TryLoggingController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string TestLogging(string id)
        {
            _logger.LogError(id);
            return $"Sent Message:{id}";
        }
        [HttpGet]
        public string TestException()
        {
            throw new Exception("This is expected exception");
        }
    }
}

