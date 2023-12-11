using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages
{
    public partial class Categories : ComponentBase, IDisposable
    {
        [Inject]
        public required CategoriesViewModel ViewModel { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ViewModel.PropertyChangedAsync -= ViewModel_PropertyChangedAsync;
        }

        protected override async Task OnInitializedAsync()
        {
            ViewModel.PropertyChangedAsync += ViewModel_PropertyChangedAsync;
            await ViewModel.InitializeAsync();
            await base.OnInitializedAsync();
        }

        private async Task ViewModel_PropertyChangedAsync(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
