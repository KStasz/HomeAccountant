using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.BillingPeriods
{
    partial class BillingPeriodChart : ComponentBase, IDisposable, IAsyncDisposable
    {
        [Parameter]
        public BillingPeriodChartViewModel? ViewModel { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            if (ViewModel is null)
                return;

            ViewModel.PropertyChangedAsync += ViewModel_PropertyChangedAsync;
            base.OnInitialized();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (ViewModel is null)
                return;

            ViewModel.PropertyChangedAsync -= ViewModel_PropertyChangedAsync;
            ViewModel.Dispose();
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (ViewModel is null)
                return;

            await ViewModel.DisposeAsync().ConfigureAwait(false);
        }

        private async Task ViewModel_PropertyChangedAsync(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
