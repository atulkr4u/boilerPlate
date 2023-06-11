using System.Text.Json;
using boilerPlate.Infra.ServiceContracts;
using StackExchange.Redis;

namespace boilerPlate.Infra.Services;
public class CachingService: ICachingService
{
    private readonly IDatabase _redisDb;
    public CachingService(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }
    public void Set(string key, string value)
    {
        _redisDb.StringSet(key, value);
    }
    public string Get(string key)
    {
        return _redisDb.StringGet(key);
    }
    public void Dispose(string key)
    {
        _redisDb.KeyDelete(key);
    }
    public void SetHash(string key, string value)
    {
        _redisDb.HashSet(key, ConvertJsonToHashEntries(value));
    }
    public string GetHash(string hash, string key)
    {
        return _redisDb.HashGet(hash, key);
    }
    static HashEntry[] ConvertJsonToHashEntries(string jsonString)
    {
        // Parse the JSON string into a JsonDocument
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);

        List<HashEntry> hashEntries = new List<HashEntry>();

        // Convert the JSON properties to HashEntry objects
        foreach (JsonProperty property in jsonDocument.RootElement.EnumerateObject())
        {
            string name = property.Name;
            string value = property.Value.ToString();

            hashEntries.Add(new HashEntry(name, value));
        }

        return hashEntries.ToArray();
    }
}

