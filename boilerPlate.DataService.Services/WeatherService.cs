using System.Xml.Linq;
using boilerPlate.DataService.Contracts;
using boilerPlate.Infra.ServiceContracts;

namespace boilerPlate.DataService.Services;
public class WeatherService : BaseDataService, IWeatherService
{
    public WeatherService(ICachingService cachingService):base(cachingService)
    {

    }
    public int GetCurrentTemprature(string cityName)
    {
        return GetData<int>(cityName, () =>
        {
            if(cityName == "delhi")
            {
                return 44;
            }
            else
            {
                return 6;
            }
        });
    }

}

