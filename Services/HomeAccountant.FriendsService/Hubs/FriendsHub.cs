using Microsoft.AspNetCore.SignalR;

namespace HomeAccountant.FriendsService;

public class FriendsHub : Hub
{
    private readonly string _refreshFriendshipCollection = "RefreshFriendshipCollection";
    
    public async Task FriendshipRequestCreated()
    {
        await Clients.All.SendAsync(_refreshFriendshipCollection);
    }
}
