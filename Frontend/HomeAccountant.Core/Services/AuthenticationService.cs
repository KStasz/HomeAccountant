using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Exceptions;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private const string LOGIN_ENDPOINT = "api/Authentication/Login";
        private const string REGISTER_ENDPOINT = "api/Authentication/Register";
        private readonly HttpClient _httpClient;
        private ITypeMapper<TokenAuthenticationModel, LoginResponseDTO> _mapper;

        public AuthenticationService(
            IHttpClientFactory httpClientFactory,
            ITypeMapper<TokenAuthenticationModel, LoginResponseDTO> mapper
            )
        {
            _httpClient = httpClientFactory.CreateClient("UnauhorizedHttpClient");
            _mapper = mapper;
        }

        public async Task<ServiceResponse<LoginResponseDTO>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(LOGIN_ENDPOINT, new
                {
                    email,
                    password,
                }, cancellationToken);

                var loginResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<LoginResponseDTO>>();
                
                return loginResponse.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResponseDTO>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse<TokenAuthenticationModel>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "/api/Authentication/RefreshToken";

                var response = await _httpClient.PostAsJsonAsync(url, new
                {
                    token,
                    refreshToken
                }, cancellationToken);

                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<LoginResponseDTO>>(cancellationToken);

                if (responseContent is null)
                {
                    throw new ServiceException("Wystąpił problem z połączeniem");
                }

                var tokenAuthentication = _mapper.Map(responseContent.Value!);

                return new ServiceResponse<TokenAuthenticationModel>(tokenAuthentication.Protect());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<TokenAuthenticationModel>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse<LoginResponseDTO>> RegisterAsync(string email, string username, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(REGISTER_ENDPOINT, new
                {
                    email,
                    username,
                    password
                }, cancellationToken);

                var registerResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<LoginResponseDTO>>(cancellationToken);

                return registerResponse.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResponseDTO>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }
    }
}
