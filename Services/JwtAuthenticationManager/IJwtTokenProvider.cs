using JwtAuthenticationManager.Model;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthenticationManager
{
    public interface IJwtTokenProvider
    {
        Task<JwtTokenPair> GenerateJwtToken(IdentityUser user);
        Task<JwtTokenPair?> VerifyAndGenerateToken(string token, string refreshToken);
    }
}