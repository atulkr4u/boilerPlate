namespace boilerPlate.Infra.ServiceContracts;
public interface ICachingService
{
    void Set(string key, string value);
    string Get(string key);
    void Dispose(string key);
    void SetHash(string key, string value);
    string GetHash(string hash, string key);
}

