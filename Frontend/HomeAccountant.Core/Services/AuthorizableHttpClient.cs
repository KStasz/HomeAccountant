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

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string uri, T value, CancellationToken cancellationToken = default)
        {
            await TrySetAuthorizationToken(cancellationToken);

            return await _client.PostAsJsonAsync(uri, value, cancellationToken);
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string uri, T value, CancellationToken cancellationToken = default)
        {
            await TrySetAuthorizationToken(cancellationToken);

            return await _client.PutAsJsonAsync(uri, value, cancellationToken);
        }

        public async Task<HttpResponseMessage> PutAsync(string uri, CancellationToken cancellationToken = default)
        {
            await TrySetAuthorizationToken(cancellationToken);

            return await _client.PutAsync(uri, null, cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string uri, CancellationToken cancellationToken = default)
        {
            await TrySetAuthorizationToken(cancellationToken);

            return await _client.DeleteAsync(uri, cancellationToken);
        }

        public async Task<HttpResponseMessage> GetAsync(string uri, CancellationToken cancellationToken = default)
        {
            await TrySetAuthorizationToken(cancellationToken);

            return await _client.GetAsync(uri, cancellationToken);
        }

        private async Task TrySetAuthorizationToken(CancellationToken cancellationToken = default)
        {
            var token = await _tokenStorageAccessor.GetTokenAsync(cancellationToken);

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
                var refreshingResult = await RefreshToken(token, cancellationToken);

                if (!refreshingResult)
                    return;

                await TrySetAuthorizationToken(cancellationToken);

                return;
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AUTHORIZATION_SCHEMA, token.Token);
        }

        private async Task<bool> RefreshToken(TokenAuthenticationModel token, CancellationToken cancellationToken = default)
        {
            var result = await _authenticationService.RefreshTokenAsync(token.Token, token.RefreshToken, cancellationToken);

            if (!result.Result || result.Value is null)
            {
                await _tokenStorageAccessor.RemoveTokenAsync(cancellationToken);
                _jwtAuthenticationStateProvider.StateChanged();

                return false;
            }

            await _tokenStorageAccessor.SetTokenAsync(result.Value, cancellationToken);
            return true;
        }
    }
}
