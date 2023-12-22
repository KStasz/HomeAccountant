using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface ITokenStorageAccessor
    {
        Task<TokenAuthenticationModel?> GetTokenAsync(CancellationToken cancellationToken = default);
        Task SetTokenAsync(TokenAuthenticationModel tokenAuthenticationModel, CancellationToken cancellationToken = default);
        Task ClearAsync(CancellationToken cancellationToken = default);
        Task RemoveTokenAsync(CancellationToken cancellationToken = default);
    }
}
