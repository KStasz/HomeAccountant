using HomeAccountant.Core.DTOs.Friends;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly AuthorizableHttpClient _httpClient;
        private readonly ILogger<FriendshipService> _logger;
        private readonly ITypeMapper<FriendshipModel, FriendshipReadDto> _friendshipMapper;

        public FriendshipService(
            AuthorizableHttpClient httpClient,
            ILogger<FriendshipService> logger,
            ITypeMapper<FriendshipModel, FriendshipReadDto> friendshipMapper)
        {
            _httpClient = httpClient;
            _logger = logger;
            _friendshipMapper = friendshipMapper;
        }

        public async Task<ServiceResponse<IEnumerable<FriendshipModel>?>> GetFriendships(string? email = null, string? name = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Friendship?email={email}&name={name}";
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<IEnumerable<FriendshipReadDto>?>>();

                if (responseContent is null)
                    return new ServiceResponse<IEnumerable<FriendshipModel>?>(
                        false,
                        ["Reading friendships failed"]);

                if (!responseContent.Result)
                    return new ServiceResponse<IEnumerable<FriendshipModel>?>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse<IEnumerable<FriendshipModel>?>(
                    responseContent.Value?.Select(_friendshipMapper.Map));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Environment.NewLine}--> {ex.Message}{Environment.NewLine}");

                return new ServiceResponse<IEnumerable<FriendshipModel>?>(
                    false,
                    ["Reading friendships failed"]);
            }
        }

        public async Task<ServiceResponse> CreateFriendship(string recipientEmail, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "/api/Friendship/CreateFriendshipRequest";
                var response = await _httpClient.PostAsJsonAsync(url, recipientEmail, cancellationToken);
                var message = await response.Content.ReadAsStringAsync();
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>();

                if (responseContent is null)
                    return new ServiceResponse(
                        false,
                        ["Unable to send friend request"]);

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{Environment.NewLine}--> {ex.Message}{Environment.NewLine}");

                return new ServiceResponse(
                    false,
                    ["Unable to send friend request"]);
            }
        }

        public async Task<ServiceResponse> DeleteFriendship(int friendshipId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Friendship?friendshipId={friendshipId}";
                var response = await _httpClient.DeleteAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>();

                if (responseContent is null)
                    return new ServiceResponse(
                        false,
                        new List<string>()
                        {
                            "Delete friendship failed"
                        });

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return new ServiceResponse(
                    false,
                    new List<string>()
                    {
                        "Delete friendship failed"
                    });
            }

            throw new NotImplementedException();
        }

        public async Task<ServiceResponse> AcceptFriendship(int friendshipId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Friendship/AcceptFriendship?friendshipId={friendshipId}";
                var response = await _httpClient.PutAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);
                
                if (responseContent is null)
                    return new ServiceResponse(
                        false,
                        new List<string>()
                        {
                            "Accepting friendship failed"
                        });

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return new ServiceResponse(
                    false,
                    ["Accepting friendship failed"]);
            }
        }
    }
}
