using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages
{
    public partial class BookBillingPeriod : ComponentBase, IDisposable
    {
        [Inject]
        public required BillingPeriodViewModel ViewModel { get; set; }

        [Parameter]
        public int RegisterId { get; set; } = 8;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ViewModel.PropertyChangedAsync -= ViewModel_PropertyChangedAsync;
        }

        protected override async Task OnParametersSetAsync()
        {
            await ViewModel.InitializeAsync(RegisterId);
            await base.OnParametersSetAsync();
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
