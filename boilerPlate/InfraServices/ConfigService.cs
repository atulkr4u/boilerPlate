using System;
using boilerPlate.Infra.ServiceContracts;

namespace boilerPlate.InfraServices
{
	public class ConfigService : IConfigService
    {
        IConfiguration _config;
        public ConfigService(IConfiguration config)
        {
            _config = config;
        }
        public string Get(string key)
        {
            var env = _config.GetSection("Environment").Value.ToString();
            return _config.GetSection($"{key}:{env}").Value.ToString();
        }
    }
}

