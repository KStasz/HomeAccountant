using Domain.Dtos.IdentityPlatform;
using Domain.Model;

namespace HomeAccountant.AccountingService.Services
{
    public class AccountInfoService : IAccountInfoService
    {
        private readonly HttpClient _httpClient;

        public AccountInfoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<UserUsernameReadDto[]>> GetUsersData(string[] userIds)
        {
            try
            {
                var url = "/api/AccountInfo/GetUserNames";
                var response = await _httpClient.PostAsJsonAsync(url, userIds);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<UserUsernameReadDto[]>>();

                return responseContent ?? throw new ArgumentNullException(nameof(responseContent));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserUsernameReadDto[]>(
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }
    }
}
