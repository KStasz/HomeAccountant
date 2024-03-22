using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IFriendsRealTimeService : IAsyncDisposable
    {
        delegate Task FriendshipCreatedHandler(object sender, RealTimeEventArgs e);
        event FriendshipCreatedHandler FriendshipCreated;
        Task InitializeAsync(CancellationToken cancellationToken = default);
        Task FriendshipCreatedAsync(CancellationToken cancellationToken = default);
    }
}
