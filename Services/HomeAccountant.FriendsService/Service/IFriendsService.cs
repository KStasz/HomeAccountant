using HomeAccountant.FriendsService.Model;

namespace HomeAccountant.FriendsService.Service
{
    public interface IFriendsService : IDbContextStandard
    {
        void CreateFriendship(Friend friend);
        IEnumerable<Friend> GetFriends(string userId);
        void DeleteFriendship(Friend friend);
        Friend? GetFriend(Func<Friend, bool> filter);
    }
}
