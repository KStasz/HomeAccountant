using AutoMapper;
using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

        public async Task<ServiceResponse<TokenAuthenticationModel>> Login(string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(LOGIN_ENDPOINT, new
                {
                    email,
                    password,
                });

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

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
            });

            var responseContent = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

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

        public async Task<ServiceResponse<TokenAuthenticationModel>> Register(string email, string username, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(REGISTER_ENDPOINT, new
                {
                    email,
                    username,
                    password
                });

                var registerResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

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
