using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HomeAccountant.Pages.Authentication
{
    public partial class LoginPage : ComponentBase, IDisposable
    {
        [Inject]
        public required LoginViewModel ViewModel { get; set; }

        protected override Task OnInitializedAsync()
        {
            ViewModel.PropertyChangedAsync += ViewModel_PropertyChangedAsync;
            return base.OnInitializedAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private async Task ViewModel_PropertyChangedAsync(object? sender, PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        protected virtual void Dispose(bool disposing) 
        {
            ViewModel.PropertyChangedAsync -= ViewModel_PropertyChangedAsync;
        }
    }
}
