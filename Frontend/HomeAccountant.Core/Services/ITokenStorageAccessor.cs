using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface ITokenStorageAccessor
    {
        Task<TokenAuthenticationModel?> GetTokenAsync();
        Task SetTokenAsync(TokenAuthenticationModel tokenAuthenticationModel);
        Task ClearAsync();
        Task RemoveTokenAsync();
    }
}
