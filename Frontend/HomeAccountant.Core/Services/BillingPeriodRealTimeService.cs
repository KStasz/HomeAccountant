using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public class BillingPeriodRealTimeService : IBillingPeriodRealTimeService
    {
        private const string REFRESH_PERIOD_ENTRIES_COLLECTION = "RefreshPeriodEntriesCollection";
        private const string ENTRY_HAS_BEEN_CREATED = "EntryHasBeenCreated";
        private const string BILLING_PERIOD_STATE_CHANGED = "BillingPeriodStateChanged";
        private const string REFRESH_PERIOD_STATE = "RefreshPeriodState";

        private readonly ISignalRHubConnection _signalRHubConnection;

        public BillingPeriodRealTimeService(ISignalRHubConnection signalRHubConnection)
        {
            _signalRHubConnection = signalRHubConnection;
        }

        public event IBillingPeriodRealTimeService.EntryCreatedHandler? EntryCreated;
        public event IBillingPeriodRealTimeService.EntryCreatedHandler? BillingPeriodStateChanged;

        public Task EntryCreatedAsync(int billingPeriodId, CancellationToken cancellationToken = default) =>
            _signalRHubConnection.SendAsync(ENTRY_HAS_BEEN_CREATED, billingPeriodId, cancellationToken);
        
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _signalRHubConnection.On<int, Task>(
                REFRESH_PERIOD_ENTRIES_COLLECTION,
                (val) =>
                {
                    if (EntryCreated is null)
                        return Task.CompletedTask;

                    return EntryCreated.Invoke(this, new RealTimeEventArgs<int>(val));
                });

            _signalRHubConnection.On<int, Task>(
                REFRESH_PERIOD_STATE,
                (val) =>
                {
                    if (BillingPeriodStateChanged is null)
                        return Task.CompletedTask;

                    return BillingPeriodStateChanged.Invoke(this, new RealTimeEventArgs<int>(val));
                });

            return _signalRHubConnection.StartAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_signalRHubConnection is not null)
            {
                await _signalRHubConnection.DisposeAsync().ConfigureAwait(false);
            }
        }

        public Task BillingPeriodStateChangedAsync(int billingPeriodId, CancellationToken cancellationToken = default) => 
            _signalRHubConnection.SendAsync(BILLING_PERIOD_STATE_CHANGED, billingPeriodId, cancellationToken);
    }
}
