using HomeAccountant.Core.Authentication;
using HomeAccountant.Core.Model;
using System.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;

namespace HomeAccountant.Core.Services
{
    public class AuthorizableHttpClient
    {
        private readonly HttpClient _client;
        private readonly ITokenStorageAccessor _tokenStorageAccessor;
        private readonly IAuthenticationService _authenticationService;
        private readonly IJwtTokenParser _jwtTokenParser;
        private readonly JwtAuthenticationStateProvider _jwtAuthenticationStateProvider;
        private const string AUTHORIZATION_SCHEMA = "Bearer";


        public AuthorizableHttpClient(HttpClient client,
            ITokenStorageAccessor tokenStorageAccessor,
            IAuthenticationService authenticationService,
            IJwtTokenParser jwtTokenParser,
            JwtAuthenticationStateProvider jwtAuthenticationStateProvider)
        {
            _client = client;
            _tokenStorageAccessor = tokenStorageAccessor;
            _authenticationService = authenticationService;
            _jwtTokenParser = jwtTokenParser;
            _jwtAuthenticationStateProvider = jwtAuthenticationStateProvider;
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string uri, T value)
        {
            await TrySetAuthorizationToken();

            return await _client.PostAsJsonAsync(uri, value);
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string uri, T value)
        {
            await TrySetAuthorizationToken();

            return await _client.PutAsJsonAsync(uri, value);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            await TrySetAuthorizationToken();

            return await _client.DeleteAsync(uri);
        }

        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            await TrySetAuthorizationToken();

            return await _client.GetAsync(uri);
        }

        private async Task TrySetAuthorizationToken()
        {
            var token = await _tokenStorageAccessor.GetTokenAsync();

            if (token is null)
            {
                return;
            }

            var expirationDate = _jwtTokenParser.GetTokenExpirationDate(token.Token);

            if (expirationDate is null)
            {
                return;
            }

            if (expirationDate < DateTime.UtcNow)
            {
                var refreshingResult = await RefreshToken(token);

                if (!refreshingResult)
                    return;

                await TrySetAuthorizationToken();

                return;
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AUTHORIZATION_SCHEMA, token.Token);
        }

        private async Task<bool> RefreshToken(TokenAuthenticationModel token)
        {
            var result = await _authenticationService.RefreshTokenAsync(token.Token, token.RefreshToken);

            if (!result.IsSucceed || result.Result is null)
            {
                await _tokenStorageAccessor.RemoveTokenAsync();
                _jwtAuthenticationStateProvider.StateChanged();

                return false;
            }

            await _tokenStorageAccessor.SetTokenAsync(result.Result);
            return true;
        }
    }
}
