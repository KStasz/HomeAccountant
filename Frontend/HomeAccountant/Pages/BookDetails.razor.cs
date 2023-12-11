using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages
{
    public partial class BookDetails : ComponentBase, IDisposable
    {
        [Parameter]
        public int RegisterId { get; set; }

        [Inject]
        public required RegisterPositionsViewModel ViewModel { get; set; }
        

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

        protected override async Task OnInitializedAsync()
        {
            ViewModel.PropertyChangedAsync += ViewModel_PropertyChangedAsync;

            await base.OnInitializedAsync();
        }

        private async Task ViewModel_PropertyChangedAsync(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
