using Domain.Dtos.FriendsService;
using Domain.Dtos.IdentityPlatform;

namespace HomeAccountant.FriendsService.Service
{
    public interface IIdentityPlatformService
    {
        Task<string?> GetUserIdByEmailAsync(string email);

        Task<UserModelDto[]?> GetUsersAsync(string[] userIds);
    }
}
