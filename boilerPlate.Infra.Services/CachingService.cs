using System.Security.AccessControl;
using System.Text.Json;
using boilerPlate.Infra.ServiceContracts;
using StackExchange.Redis;
using System.Runtime.Caching;
using boilerPlate.Domain.Helpers;
using Microsoft.Extensions.Logging;
using boilerPlate.Domain.DomainBase;

namespace boilerPlate.Infra.Services;
public class CachingService: ICachingService
{
    IDatabase _readDatabase;
    IDatabase _writeDatabase;
    static string _readDb;
    static string _writeDb;
    TimeSpan _slidingExpiry;
    ObjectCache _memoryCache;
    IConfigService _configService;
    ILogger<CachingService> _logger;
    public CachingService(IConfigService configService, ILogger<CachingService> logger)
    {
        _configService = configService;
        _memoryCache = MemoryCache.Default;
        _slidingExpiry = new TimeSpan(0, _configService.Get("SlidingExpiryInMin").ToSafeInt(), 0);
        _readDb = _configService.Get("RedisDbRead");
        _writeDb = _configService.Get("RedisDbWrite");
        _logger = logger;
        TryConnect();

    }
    public void TryConnect()
    {
        try
        {
            WriteDb.StringSet("App", "Hello");
            var test = ReadDb.StringGet("App");
        }
        catch (Exception ex)
        {
            _logger.LogCritical("Failed to Connect to Redis");
        }
    }
    public string InternalGet(string key)
    {
        if (!_memoryCache.Contains(key))
        {
            return null;
        }
        return _memoryCache.Get(key).ToString();
    }
    public void InternalClear(string key)
    {
        _memoryCache.Remove(key);
    }
    public void InternalSet(string key, string value)
    {
        var expirationTime = DateTimeOffset.Now.AddMinutes(_configService.Get("SlidingExpiryInMin").ToSafeInt());
        _memoryCache.Set(key, value, expirationTime);
    }
    public void InternalSetWithExpiry(string key, string value, DateTimeOffset expirationTime)
    {
        _memoryCache.Set(key, value, expirationTime);
    }
    public string Get(string key)
    {
        try
        {
            var result = ReadDb.StringGet(key);
            return result;
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _logger.LogError($"Failed to fetch key '{key}' from Redis");
            }
            return null;
        }
    }
    public BaseResponse GetWithResponse(string key)
    {
        try
        {
            var result = ReadDb.StringGet(key);
            return new BaseResponse { IsSuccess = true, ReturnMessage = result };
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _logger.LogError($"Failed to fetch key '{key}' from Redis");
                return new BaseResponse { IsSuccess = false, ReturnMessage = ex.Message };
            }
            return null;
        }
    }

    public void Set(string key, string value)
    {
        try
        {
            Guid outresult;
            if (Guid.TryParse(key, out outresult))
            {
                throw new ApplicationException($"GUID Value not valid for Redis{key}:{value}");
            }

            WriteDb.StringSetAsync(key, value, _slidingExpiry);
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _logger.LogError($"Failed to set key '{key}' value '{value}' in Redis");
            }
        }
    }
    public void Set(string key, string value, int expiry)
    {
        try
        {
            Guid outresult;
            if (Guid.TryParse(key, out outresult))
            {
                throw new ApplicationException($"GUID Value not valid for Redis{key}:{value}");
            }


            WriteDb.StringSetAsync(key, value, new TimeSpan(0, 0, expiry));
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _logger.LogError($"Failed to set key '{key}' value '{value}' in Redis");
            }
        }
    }
    public void Update(string key, string value, int expiry)
    {
        try
        {
            if (ReadDb.KeyTimeToLive(key).HasValue)
            {
                WriteDb.StringSetAsync(key, value, ReadDb.KeyTimeToLive(key).Value);
            }
            else
            {
                Set(key, value, expiry);
            }
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _logger.LogError($"Failed to update key '{key}' value '{value}' in Redis");
            }
        }
    }
    public int ExpiryRemaining(string key)
    {
        if (!ReadDb.KeyTimeToLive(key).HasValue)
        {
            return 0;
        }
        else
        {
            return (int)ReadDb.KeyTimeToLive(key).Value.TotalMinutes;
        }
    }
    public bool Remove(string key)
    {
        try
        {
            return WriteDb.KeyDelete(key);
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _logger.LogError($"Failed to remove key '{key}' from Redis");
            }
            return false;
        }

    }
    public List<string> GetAllKeys()
    {
        Guid guid;
        return ConnectionMultiplexer.Connect(_readDb).GetServer(_readDb).Keys().Select(t => t.ToString()).Where(t =>
        !t.StartsWith("EmpToken")
        ).ToList();
    }
    public List<string> GetAllInternalKeys()
    {
        return _memoryCache.ToList().Select(t => t.Key).ToList();
    }

    public bool FindValueInTheCache(string prefix, string findValue)
    {
        bool response = false;
        var keys = ConnectionMultiplexer.Connect(_readDb).GetServer(_readDb).Keys().Select(t => t.ToString()).Where(t =>
        t.StartsWith(prefix));
        if (keys != null)
        {
            var keylist = keys.ToList();
            foreach (string cacheKey in keylist)
            {
                var cache = Get(cacheKey);
                if (cache != null && cache.Contains(findValue))
                {
                    response = true;
                }
            }
        }
        return response;
    }

    public void CleanByPreFix(string prefix)
    {
        var keys = ConnectionMultiplexer.Connect(_readDb).GetServer(_readDb).Keys().Select(t => t.ToString()).Where(t =>
        t.StartsWith(prefix)).ToList();
        foreach (var k in keys)
        {
            Remove(k);
        }
    }
    private IDatabase ReadDb
    {
        get
        {
            return readLazyConnection.Value.GetDatabase();
        }
    }
    private IDatabase WriteDb
    {
        get
        {
            return writeLazyConnection.Value.GetDatabase();
        }
    }
    private Lazy<ConnectionMultiplexer> readLazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
        return ConnectionMultiplexer.Connect(_readDb);
    });
    private Lazy<ConnectionMultiplexer> writeLazyConnection = new Lazy<ConnectionMultiplexer>(() =>
    {
        return ConnectionMultiplexer.Connect(_writeDb);
    });
    public void Dispose(string key)
    {
        WriteDb.KeyDelete(key);
    }
    public void SetHash(string key, string value)
    {
        WriteDb.HashSet(key, ConvertJsonToHashEntries(value));
    }
    public string GetHash(string hash, string key)
    {
        return ReadDb.HashGet(hash, key);
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

