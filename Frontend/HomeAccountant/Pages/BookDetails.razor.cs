using HomeAccountant.Core.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages
{
    public partial class BookDetails : ComponentBase, IDisposable
    {
        [Parameter]
        public int BillingPeriodId { get; set; }

        [Parameter]
        public int RegisterId { get; set; }

        [Inject]
        public required RegisterPositionsViewModel ViewModel { get; set; }

#pragma warning disable BL0007
        [Parameter]
        public bool IsBillingPeriodOpen
        {
            get
            {
                return ViewModel.IsBillingPeriodOpen;
            }
            set
            {
                ViewModel.IsBillingPeriodOpen = value;
            }
        }
#pragma warning restore BL0007


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            var xd = ViewModel.IsBillingPeriodOpen ? new Dictionary<string, object>() : new Dictionary<string, object>() { { "disabled", "" } };
            ViewModel.PropertyChangedAsync -= ViewModel_PropertyChangedAsync;
        }

        protected override async Task OnParametersSetAsync()
        {
            await ViewModel.InitializeAsync(RegisterId, BillingPeriodId);

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
