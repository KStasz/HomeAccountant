using HomeAccountant.FriendsService.Data;
using HomeAccountant.FriendsService.Model;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HomeAccountant.FriendsService.Service
{
    public class FriendRequestsService : IFriendRequestsService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FriendRequestsService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void CreateRequest(FriendRequest friendRequest)
        {
            var result = _applicationDbContext.FriendRequests.Add(friendRequest);
        }

        public FriendRequest? GetRequest(int requestId)
        {
            return _applicationDbContext.FriendRequests.FirstOrDefault(x => x.Id == requestId);
        }

        public IEnumerable<FriendRequest> GetRequests(Expression<Func<FriendRequest, bool>> predicate)
        {
            return _applicationDbContext.FriendRequests
                .Where(predicate);
        }

        public FriendRequest? SearchRequest(Func<FriendRequest, bool> search)
        {
            return _applicationDbContext.FriendRequests.FirstOrDefault(search);
        }

        public void UpdateRequest(FriendRequest friendRequest)
        {
            _applicationDbContext.FriendRequests.Update(friendRequest);
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
