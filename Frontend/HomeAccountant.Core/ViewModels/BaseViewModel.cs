﻿using HomeAccountant.Core.Delegates;
using HomeAccountant.Core.Interfaces;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HomeAccountant.Core.ViewModels
{
    public class BaseViewModel : INotifyPropertyChangedAsync, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangedEventHandlerAsync? PropertyChangedAsync;
        private CancellationTokenSource? CancellationTokenSource;

        protected CancellationToken CancellationToken => (CancellationTokenSource ??= new()).Token;
        
        protected void NotifyPropertyChangedAsync([CallerMemberName] string propertyName = "") => PropertyChangedAsync?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void NotifyPropertyChangedSync([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            NotifyPropertyChangedAsync(propertyName);
            NotifyPropertyChangedSync(propertyName);
        }

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
                NotifyPropertyChangedSync();
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
        }
    }
}
