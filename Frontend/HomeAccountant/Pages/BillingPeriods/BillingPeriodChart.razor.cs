using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.BillingPeriods
{
    partial class BillingPeriodChart
    {
        [Parameter]
        public int BillingPeriodId { get; set; } = 3;

        [Parameter]
        public int Registerid { get; set; } = 13;

        [Parameter]
        public string? BillingPeriodName { get; set; } = "Test2";
    }
}
