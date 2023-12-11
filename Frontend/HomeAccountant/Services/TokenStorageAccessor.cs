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

        public async Task ClearAsync()
        {
            await _jSRuntime.InvokeVoidAsync("clearToken");
        }

        public async Task<TokenAuthenticationModel?> GetTokenAsync()
        {
            var result = await _jSRuntime.InvokeAsync<string>("getToken");
            
            if (result is null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<TokenAuthenticationModel?>(result);
        }

        public async Task RemoveTokenAsync()
        {
            await _jSRuntime.InvokeVoidAsync("removeToken");
        }

        public async Task SetTokenAsync(TokenAuthenticationModel tokenAuthenticationModel)
        {
            var token = JsonSerializer.Serialize(tokenAuthenticationModel);
            await _jSRuntime.InvokeVoidAsync("setToken", token);
        }
    }
}
