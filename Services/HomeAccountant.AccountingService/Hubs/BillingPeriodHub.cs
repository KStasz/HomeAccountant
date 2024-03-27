using Microsoft.AspNetCore.SignalR;

namespace HomeAccountant.AccountingService.Hubs
{
    public class BillingPeriodHub : Hub
    {
        private const string REFRESH_PERIOD_ENTRIES_COLLECTION = "RefreshPeriodEntriesCollection";
        private const string REFRESH_PERIOD_STATE = "RefreshPeriodState";
        
        public Task EntryHasBeenCreated(int billingPeriodId) => 
            Clients.All.SendAsync(REFRESH_PERIOD_ENTRIES_COLLECTION, billingPeriodId);
        
        public Task BillingPeriodStateChanged(int billingPeriodId) => 
            Clients.All.SendAsync(REFRESH_PERIOD_STATE, billingPeriodId);
    }
}
