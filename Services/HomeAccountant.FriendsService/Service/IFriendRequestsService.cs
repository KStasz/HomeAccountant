using HomeAccountant.FriendsService.Model;
using System.Linq.Expressions;

namespace HomeAccountant.FriendsService.Service
{
    public interface IFriendRequestsService
    {
        void CreateRequest(FriendRequest friendRequest);
        void UpdateRequest(FriendRequest friendRequest);
        void DeleteRequest(FriendRequest friendRequest);
        FriendRequest? GetRequest(int requestId);
        IEnumerable<FriendRequest> GetRequests(Expression<Func<FriendRequest, bool>> predicate);
        FriendRequest? SearchRequest(Func<FriendRequest, bool> search);
        Task SaveChangesAsync();
    }
}
