using HomeAccountant.FriendsService.Model;

namespace HomeAccountant.FriendsService.Service
{
    public interface IFriendshipCreator
    {
        void CreateFriendship(FriendRequest request);
    }
}
