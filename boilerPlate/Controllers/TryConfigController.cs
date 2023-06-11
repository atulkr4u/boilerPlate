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
    public class TryConfigController : Controller
    {
        private readonly ILogger<TryConfigController> _logger;
        IConfigService _config;
        public TryConfigController(ILogger<TryConfigController> logger, IConfigService config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public string GetKey(string id)
        {
            var result =  _config.Get(id);
            return result;
        }
        
    }
}

