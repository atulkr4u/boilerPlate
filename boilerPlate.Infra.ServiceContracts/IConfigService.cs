using System;
namespace boilerPlate.Infra.ServiceContracts
{
	public interface IConfigService
    {
		string Get(string key);
        string CurrentEnv();

    }
}

