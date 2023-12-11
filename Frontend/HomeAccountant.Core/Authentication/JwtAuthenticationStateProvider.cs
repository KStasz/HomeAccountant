using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace HomeAccountant.Core.Authentication
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string AUTHENTICATION_TYPE = "jwt";
        private readonly ITokenStorageAccessor _tokenStorage;
        private readonly IJwtTokenParser _jwtTokenParser;

        public JwtAuthenticationStateProvider(ITokenStorageAccessor tokenStorage,
            IJwtTokenParser jwtTokenParser)
        {
            _tokenStorage = tokenStorage;
            _jwtTokenParser = jwtTokenParser;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _tokenStorage.GetTokenAsync();

            if (token is null)
            {
                return new AuthenticationState(new ClaimsPrincipal());
            }

            var identity = new ClaimsIdentity(_jwtTokenParser.ParseClaimsFromJwt(token.Token), AUTHENTICATION_TYPE);

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void StateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
