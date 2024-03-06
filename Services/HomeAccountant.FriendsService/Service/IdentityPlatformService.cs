using Domain.Dtos.IdentityPlatform;
using Domain.Model;
using System.Text;
using System.Text.Json;

namespace HomeAccountant.FriendsService.Service
{
    public class IdentityPlatformService : IIdentityPlatformService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IdentityPlatformService> _logger;

        public IdentityPlatformService(
            HttpClient httpClient,
            ILogger<IdentityPlatformService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResponse<string?>> GetEmailByUserId(string userId)
        {
            try
            {
                string url = $"/api/Account/GetEmailByUserId/{userId}";

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<string?>>();

                if (responseContent is null)
                    return new ServiceResponse<string?>(
                        new List<string>()
                        {
                            "Unable to read user Email"
                        });

                return responseContent;

            }
            catch (Exception ex)
            {
                _logger.LogError($"{Environment.NewLine}--> {ex.Message}{Environment.NewLine}");

                return new ServiceResponse<string?>(
                    new List<string>()
                    {
                        "Unable to read user Email"
                    });
            }
        }

        public async Task<string?> GetUserIdByEmailAsync(string email)
        {
            string url = $"api/Account/GetUserId/{email}";

            var response = await _httpClient.GetAsync(url);


            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<ServiceResponse<UserModelDto[]?>> GetUsersAsync(string[] userIds)
        {
            try
            {
                string url = "api/Account/GetUsers";
                var stringContent = new StringContent(
                    JsonSerializer.Serialize(userIds),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(url, stringContent);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<UserModelDto[]?>>();

                if (responseContent is null)
                    return new ServiceResponse<UserModelDto[]?>(
                        new List<string>()
                        {
                            "Unable to read users"
                        });

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return new ServiceResponse<UserModelDto[]?>(
                    new List<string>()
                    {
                       "Unable to read users"
                    });
            }
        }
    }
}
