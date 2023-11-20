using Domain.Dtos.FriendsService;
using Domain.Dtos.IdentityPlatform;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeAccountant.FriendsService.Service
{
    public class IdentityPlatformService : IIdentityPlatformService
    {
        private readonly HttpClient _httpClient;

        public IdentityPlatformService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

        public async Task<UserModelDto[]?> GetUsersAsync(string[] userIds)
        {
            string url = "api/Account/GetUsers";

            var stringContent = new StringContent(
                JsonSerializer.Serialize(userIds),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, stringContent);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserModelDto[]>();
        }
    }
}
