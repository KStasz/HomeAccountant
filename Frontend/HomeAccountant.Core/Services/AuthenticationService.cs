using AutoMapper;
using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Model;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private const string LOGIN_ENDPOINT = "api/Authentication/Login";
        private const string REGISTER_ENDPOINT = "api/Authentication/Register";
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ILogger<AuthenticationService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("UnauhorizedHttpClient");
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<TokenAuthenticationModel>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(LOGIN_ENDPOINT, new
                {
                    email,
                    password,
                }, cancellationToken);

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    if (!loginResponse?.Result ?? false)
                    {
                        return new ServiceResponse<TokenAuthenticationModel>(false, loginResponse?.Errors);
                    }

                    return new ServiceResponse<TokenAuthenticationModel>(false);
                }

                var tokenAuthentication = _mapper.Map<TokenAuthenticationModel>(loginResponse);

                return new ServiceResponse<TokenAuthenticationModel>(tokenAuthentication);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);

                return new ServiceResponse<TokenAuthenticationModel>(false, new List<string>()
                {
                    "Wystąpił błąd"
                });
            }
        }

        public async Task<ServiceResponse<TokenAuthenticationModel>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var url = "/api/Authentication/RefreshToken";

            var response = await _httpClient.PostAsJsonAsync(url, new
            {
                token,
                refreshToken
            }, cancellationToken);

            var responseContent = await response.Content.ReadFromJsonAsync<LoginResponseDTO>(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                if (responseContent?.Result ?? false)
                {
                    return new ServiceResponse<TokenAuthenticationModel>(false, responseContent?.Errors);
                }

                return new ServiceResponse<TokenAuthenticationModel>(false);
            }

            var tokenAuthentication = _mapper.Map<TokenAuthenticationModel>(responseContent);

            return new ServiceResponse<TokenAuthenticationModel>(tokenAuthentication);
        }

        public async Task<ServiceResponse<TokenAuthenticationModel>> RegisterAsync(string email, string username, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(REGISTER_ENDPOINT, new
                {
                    email,
                    username,
                    password
                }, cancellationToken);

                var registerResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    if (!registerResponse?.Result ?? false)
                    {
                        return new ServiceResponse<TokenAuthenticationModel>(false, registerResponse?.Errors);
                    }

                    return new ServiceResponse<TokenAuthenticationModel>(false);
                }

                var tokenAuthentication = _mapper.Map<TokenAuthenticationModel>(registerResponse);

                return new ServiceResponse<TokenAuthenticationModel>(tokenAuthentication);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, ex.Message);

                return new ServiceResponse<TokenAuthenticationModel>(false, new List<string>()
                {
                    "Wystąpił błąd"
                });
            }
        }
    }
}
