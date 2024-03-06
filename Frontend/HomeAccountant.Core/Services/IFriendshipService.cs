using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IFriendshipService
    {
        Task<ServiceResponse<IEnumerable<FriendshipModel>?>> GetFriendships(string? email = null, string? name = null, CancellationToken cancellationToken = default);
        Task<ServiceResponse> CreateFriendship(string recipientEmail, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteFriendship(int friendshipId, CancellationToken cancellationToken = default);
        Task<ServiceResponse> AcceptFriendship(int friendshipId, CancellationToken cancellationToken = default);
    }
}
