using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Exceptions;
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
        private ITypeMapper<TokenAuthenticationModel, LoginResponseDto> _mapper;
        private readonly ITypeMapper<LoginResponseModel, LoginResponseDto> _loginResponseMapper;

        public AuthenticationService(
            IHttpClientFactory httpClientFactory,
            ITypeMapper<TokenAuthenticationModel, LoginResponseDto> mapper,
            ITypeMapper<LoginResponseModel, LoginResponseDto> loginResponseMapper)
        {
            _httpClient = httpClientFactory.CreateClient("UnauhorizedHttpClient");
            _mapper = mapper;
            _loginResponseMapper = loginResponseMapper;
        }

        public async Task<ServiceResponse<LoginResponseModel?>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(LOGIN_ENDPOINT, new
                {
                    email,
                    password,
                }, cancellationToken);

                var loginResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<LoginResponseDto>>();

                if (loginResponse is null)
                    throw new ServiceException("Wystąpił problem podczas odczytywania danych");

                if (!loginResponse.Result)
                    return new ServiceResponse<LoginResponseModel?>(
                        loginResponse.Result,
                        loginResponse?.Errors);

                return new ServiceResponse<LoginResponseModel?>(_loginResponseMapper.Map(loginResponse.Value));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResponseModel?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse<TokenAuthenticationModel?>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "/api/Authentication/RefreshToken";

                var response = await _httpClient.PostAsJsonAsync(url, new
                {
                    token,
                    refreshToken
                }, cancellationToken);

                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<LoginResponseDto>>(cancellationToken);

                if (responseContent is null)
                    throw new ServiceException("Wystąpił problem podczas odczytywania danych");

                if (!responseContent.Result)
                    return new ServiceResponse<TokenAuthenticationModel?>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse<TokenAuthenticationModel?>(_mapper.Map(responseContent.Value));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<TokenAuthenticationModel?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse<LoginResponseModel?>> RegisterAsync(string email, string username, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(REGISTER_ENDPOINT, new
                {
                    email,
                    username,
                    password
                }, cancellationToken);

                var registerResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<LoginResponseDto>>(cancellationToken);

                if (registerResponse is null)
                    throw new ServiceException("Wystąpił problem podczas odczytywania danych");

                if (!registerResponse.Result)
                    return new ServiceResponse<LoginResponseModel?>(
                        false,
                        registerResponse?.Errors);

                return new ServiceResponse<LoginResponseModel?>(_loginResponseMapper.Map(registerResponse.Value));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<LoginResponseModel?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }
    }
}
