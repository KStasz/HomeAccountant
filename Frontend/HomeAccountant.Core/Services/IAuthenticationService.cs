using HomeAccountant.Core.Model;
using Microsoft.Extensions.Primitives;

namespace HomeAccountant.Core.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse<TokenAuthenticationModel>> Login(string email, string password);
        Task<ServiceResponse<TokenAuthenticationModel>> Register(string email, string username, string password);
        Task<ServiceResponse<TokenAuthenticationModel>> RefreshTokenAsync(string token, string refreshToken);
    }
}
