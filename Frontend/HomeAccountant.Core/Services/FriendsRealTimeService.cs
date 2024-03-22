using HomeAccountant.Core.Model;
using Microsoft.Extensions.Logging;

namespace HomeAccountant.Core.Services
{
    public class FriendsRealTimeService : IFriendsRealTimeService
    {
        private const string _refreshFriendshipCollectionEventName = "RefreshFriendshipCollection";
        private const string _friendshipRequestCreatedEventName = "FriendshipRequestCreated";

        private readonly ISignalRHubConnection _signalRHubConnection;

        public event IFriendsRealTimeService.FriendshipCreatedHandler? FriendshipCreated;

        public FriendsRealTimeService(ISignalRHubConnection signalRHubConnection)
        {
            _signalRHubConnection = signalRHubConnection;
        }

        public Task FriendshipCreatedAsync(CancellationToken cancellationToken = default)
        {
            return _signalRHubConnection.SendAsync(_friendshipRequestCreatedEventName, cancellationToken);
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _signalRHubConnection.On(
                _refreshFriendshipCollectionEventName, 
                () => FriendshipCreated?.Invoke(this, RealTimeEventArgs.Empty));

            await _signalRHubConnection.StartAsync(cancellationToken);
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
