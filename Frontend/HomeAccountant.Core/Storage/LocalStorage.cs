using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HomeAccountant.Core.Storage
{
    public class LocalStorage : ILocalStorage
    {
        private readonly Dictionary<string, string> _storage;
        public LocalStorage()
        {
            _storage = new Dictionary<string, string>();
        }

        public T? GetItem<T>(string key)
        {
            if (!_storage.ContainsKey(key))
            {
                return default;
            }

            var value = _storage[key];

            return JsonSerializer.Deserialize<T>(value);
        }

        public void RemoveItem(string key)
        {
            if (!_storage.ContainsKey(key))
                return;

            _storage.Remove(key);
        }

        public void SetItem<T>(string key, T value)
        {
            if (_storage.ContainsKey(key))
                return;

            if (value is null)
            {
                return;
            }

            _storage.Add(key, JsonSerializer.Serialize(value));
        }
    }
}
