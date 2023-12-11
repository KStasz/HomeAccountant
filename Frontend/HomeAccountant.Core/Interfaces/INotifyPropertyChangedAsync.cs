using HomeAccountant.Core.Delegates;
using System.ComponentModel;

namespace HomeAccountant.Core.Interfaces
{
    public interface INotifyPropertyChangedAsync : INotifyPropertyChanged
    {
        event PropertyChangedEventHandlerAsync? PropertyChangedAsync;
    }
}
