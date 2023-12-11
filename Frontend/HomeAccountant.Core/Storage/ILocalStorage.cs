using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.Storage
{
    public interface ILocalStorage
    {
        void SetItem<T>(string key, T value);
        T? GetItem<T>(string key);
        void RemoveItem(string key);
    }
}
