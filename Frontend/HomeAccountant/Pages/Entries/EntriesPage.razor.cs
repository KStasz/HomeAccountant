using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.Entries
{
    partial class EntriesPage : ComponentBase, IDisposable, IAsyncDisposable
    {
        [Parameter]
        public EntryViewModel? ViewModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (ViewModel is null)
                return;

            ViewModel.PropertyChangedAsync += ViewModel_PropertyChangedAsync;
            await base.OnInitializedAsync();
        }

        private async Task ViewModel_PropertyChangedAsync(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (ViewModel is null)
                return;

            ViewModel.PropertyChangedAsync -= ViewModel_PropertyChangedAsync;
            ViewModel.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (ViewModel is null)
                return;

            await ViewModel.DisposeAsync().ConfigureAwait(false);
        }
    }
}
