using HomeAccountant.Core.Delegates;
using HomeAccountant.Core.Interfaces;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.ViewModels
{
    public class BaseViewModel : INotifyPropertyChangedAsync
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangedEventHandlerAsync? PropertyChangedAsync;

        protected void NotifyPropertyChangedAsync([CallerMemberName] string propertyName = "") => PropertyChangedAsync?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _isBusy;
        public bool IsBusy
        {
            get 
            {
                return _isBusy; 
            }
            set 
            {
                _isBusy = value;
                NotifyPropertyChanged();
                NotifyPropertyChangedAsync();
            }
        }

        private MarkupString _errorMessage;
        public MarkupString ErrorMessage
        {
            get 
            {
                return _errorMessage; 
            }
            set 
            {
                _errorMessage = value;
                NotifyPropertyChanged();
                NotifyPropertyChangedAsync();
            }
        }

        protected void SetErrorMessage(params string[] value)
        {
            ErrorMessage = (MarkupString)string.Join("<br />", value);
        }

        protected void ClearErrorMessage()
        {
            ErrorMessage = (MarkupString)string.Empty;
        }
    }
}
