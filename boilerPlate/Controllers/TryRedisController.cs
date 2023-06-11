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
    public class TryRedisController : Controller
    {
        private readonly ILogger<TryRedisController> _logger;
        ICachingService _cacheService;
        public TryRedisController(ILogger<TryRedisController> logger, ICachingService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }
        [HttpGet]
        public string AddKey(string key,string value)
        {
            _cacheService.Set(key,value);
            return "Ok";
        }
        [HttpGet]
        public string GetKey(string key)
        {
            return _cacheService.Get(key);
        }
        [HttpGet]
        public string AddHash(string key, string value)
        {
            _cacheService.SetHash(key, value);
            return "Ok";
        }
        [HttpGet]
        public string GetHashKey(string hash,string key)
        {
            return _cacheService.GetHash(hash,key);
        }
        


    }
}

