using HomeAccountant.FriendsService.Data;
using HomeAccountant.FriendsService.Model;

namespace HomeAccountant.FriendsService.Service
{
    public class FriendsService : IFriendsService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public FriendsService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public void CreateFriendship(Friend friend)
        {
            _applicationDbContext.Friends.Add(friend);
        }

        public void DeleteFriendship(Friend friend)
        {
            _applicationDbContext.Friends.Remove(friend);
        }

        public Friend? GetFriend(Func<Friend, bool> filter)
        {
            return _applicationDbContext.Friends.FirstOrDefault(filter);
        }

        public IEnumerable<Friend> GetFriends(string userId)
        {
            return _applicationDbContext.Friends.Where(x => x.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
