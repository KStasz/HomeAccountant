namespace HomeAccountant.Core.Services
{
    public class MemoryStorage : IMemoryStorage
    {
        private Dictionary<string, KeyValuePair<Type, object?>>? _storage;

        public MemoryStorage()
        {
            _storage = new Dictionary<string, KeyValuePair<Type, object?>>();
        }

        public bool Add<T>(string key, T? value)
        {
            if (_storage is null)
                return false;

            _storage.Add(key, new KeyValuePair<Type, object?>(typeof(T), value));

            return true;
        }

        public Type? GetTypeOfValue(string key)
        {
            KeyValuePair<Type, object?>? value = _storage?[key];

            return value?.Key;
        }

        public T? GetValue<T>(string key)
        {
            var value = _storage?[key];

            if (value is null)
                return default;

            return (T?)Convert.ChangeType(value?.Value, typeof(T));
        }

        public bool Remove(string key)
        {
            if (_storage is null)
                return false;

            _storage.Remove(key);

            return true;
        }

        public bool Update<T>(string key, T? value)
        {
            if (_storage is null)
            {
                return false;
            }

            _storage[key] = new KeyValuePair<Type, object?>(typeof(T), value);

            return true;
        }
    }
}
