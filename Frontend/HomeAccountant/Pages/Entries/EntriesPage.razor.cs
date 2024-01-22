using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.Entries
{
    partial class EntriesPage
    {
        [Parameter]
        public int BillingPeriodId { get; set; } = 3;

        [Parameter]
        public bool IsPeriodOpen { get; set; } = true;

        [Parameter]
        public int RegisterId { get; set; } = 13;
    }
}
