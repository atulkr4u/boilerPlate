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
            return _config.GetSection($"{key}:{CurrentEnv()}").Value.ToString();
        }
        public string CurrentEnv()
        {
           return _config.GetSection("Environment").Value.ToString();
        }
}
}

