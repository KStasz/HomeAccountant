using HomeAccountant.Core.DTOs.BillingPeriod;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.Entries
{
    partial class EntriesPage
    {
        [Parameter]
        public BillingPeriodReadDto? BillingPeriod { get; set; }

        [Parameter]
        public int RegisterId { get; set; }

        [CascadingParameter(Name = "RefreshChart")]
        public Func<Task>? RefreshChart { get; set; }

        protected override Task OnParametersSetAsync()
        {
            ViewModel.RefreshChart = RefreshChart;
            return base.OnParametersSetAsync();
        }
    }
}
