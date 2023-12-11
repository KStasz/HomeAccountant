using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages
{
    public partial class Books : ComponentBase, IDisposable
    {
        [Inject]
        public required RegisterViewModel ViewModel { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ViewModel.PropertyChangedAsync -= ViewModel_PropertyChangedAsync;
        }

        protected override void OnInitialized()
        {
            ViewModel.PropertyChangedAsync += ViewModel_PropertyChangedAsync;
            base.OnInitialized();
        }

        private async Task ViewModel_PropertyChangedAsync(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
