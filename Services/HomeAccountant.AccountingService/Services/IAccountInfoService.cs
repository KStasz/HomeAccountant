using Domain.Dtos.IdentityPlatform;
using Domain.Model;

namespace HomeAccountant.AccountingService.Services
{
    public interface IAccountInfoService
    {
        Task<ServiceResponse<UserUsernameReadDto[]>> GetUsersData(string[] userIds);
    }
}
