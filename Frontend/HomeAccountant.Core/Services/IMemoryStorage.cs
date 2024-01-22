
namespace HomeAccountant.Core.Services
{
    public interface IMemoryStorage
    {
        bool Add<T>(string key, T? value);
        Type? GetTypeOfValue(string key);
        T? GetValue<T>(string key);
        bool Remove(string key);
        bool Update<T>(string key, T? value);
    }
}