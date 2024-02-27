using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.JSInterop;
using System.Text.Json;

namespace HomeAccountant.Services
{
    public class TokenStorageAccessor : ITokenStorageAccessor
    {
        private readonly IJSRuntime _jSRuntime;

        public TokenStorageAccessor(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task ClearAsync(CancellationToken cancellationToken = default)
        {
            await _jSRuntime.InvokeVoidAsync("clearToken", cancellationToken: cancellationToken);
        }

        public async Task<TokenAuthenticationModel?> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            var result = await _jSRuntime.InvokeAsync<string>("getToken", cancellationToken);

            if (result is null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<TokenAuthenticationModel?>(result);
        }

        public async Task RemoveTokenAsync(CancellationToken cancellationToken = default)
        {
            await _jSRuntime.InvokeVoidAsync("removeToken", cancellationToken);
        }

        public async Task SetTokenAsync(LoginResponseModel tokenAuthenticationModel, CancellationToken cancellationToken = default)
        {
            var token = JsonSerializer.Serialize(tokenAuthenticationModel);
            await _jSRuntime.InvokeVoidAsync("setToken", cancellationToken, token);
        }
    }
}
