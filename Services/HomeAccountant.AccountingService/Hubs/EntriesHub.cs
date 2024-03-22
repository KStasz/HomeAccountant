using Microsoft.AspNetCore.SignalR;

namespace HomeAccountant.AccountingService.Hubs
{
    public class EntriesHub : Hub
    {
        private const string REFRESH_PERIOD_ENTRIES_COLLECTION = "RefreshPeriodEntriesCollection";
        public async Task EntryHasBeenCreated()
        {
            await Clients.All.SendAsync(REFRESH_PERIOD_ENTRIES_COLLECTION);
        }
    }
}
