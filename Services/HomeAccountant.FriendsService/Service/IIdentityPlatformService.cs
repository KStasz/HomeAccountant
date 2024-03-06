using Domain.Dtos.FriendsService;
using Domain.Dtos.IdentityPlatform;
using Domain.Model;

namespace HomeAccountant.FriendsService.Service
{
    public interface IIdentityPlatformService
    {
        Task<string?> GetUserIdByEmailAsync(string email);
        Task<ServiceResponse<string?>> GetEmailByUserId(string userId);
        Task<ServiceResponse<UserModelDto[]?>> GetUsersAsync(string[] userIds);
    }
}
