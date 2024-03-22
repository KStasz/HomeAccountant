using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IFriendsRealTimeService : IRealTimeDefaults
    {
        delegate Task FriendshipCreatedHandler(object sender, RealTimeEventArgs e);
        event FriendshipCreatedHandler? FriendshipCreated;
        Task FriendshipCreatedAsync(CancellationToken cancellationToken = default);
    }
}
