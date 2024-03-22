using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public class EntriesRealTimeService : IEntriesRealTimeService
    {
        private const string REFRESH_PERIOD_ENTRIES_COLLECTION = "RefreshPeriodEntriesCollection";
        private const string ENTRY_HAS_BEEN_CREATED = "EntryHasBeenCreated";

        private readonly ISignalRHubConnection _signalRHubConnection;

        public EntriesRealTimeService(ISignalRHubConnection signalRHubConnection)
        {
            _signalRHubConnection = signalRHubConnection;
        }

        public event IEntriesRealTimeService.EntryCreatedHandler? EntryCreated;

        public Task EntryCreatedAsync(CancellationToken cancellationToken = default)
        {
            return _signalRHubConnection.SendAsync(ENTRY_HAS_BEEN_CREATED, cancellationToken);
        }

        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _signalRHubConnection.On(
                REFRESH_PERIOD_ENTRIES_COLLECTION,
                () => EntryCreated?.Invoke(this, RealTimeEventArgs.Empty));

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
    }
}
