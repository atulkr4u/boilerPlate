using System.Text.Json;
using boilerPlate.DataService.Contracts;
using boilerPlate.Infra.ServiceContracts;
namespace boilerPlate.DataService.Services;
public class BaseDataService
{
    private readonly ICachingService _cachingService;

    public BaseDataService(ICachingService cachingService)
    {
        _cachingService = cachingService;
    }
    public T GetData<T>(string cacheKey, Func<T> fetchFromDatabase)
    {
        string dataJson = _cachingService.Get(cacheKey);
        if (dataJson != null)
        {
            // Data found in Redis, deserialize and return it
            return JsonSerializer.Deserialize<T>(dataJson);
        }

        // Data not found in Redis, fetch from the database
        T data = fetchFromDatabase();

        // Store the fetched data in Redis
        _cachingService.Set(cacheKey, JsonSerializer.Serialize(data));

        return data;
    }

}

