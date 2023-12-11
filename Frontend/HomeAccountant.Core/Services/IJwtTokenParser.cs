using System.Security.Claims;

namespace HomeAccountant.Core.Services
{
    public interface IJwtTokenParser
    {
        DateTime? GetTokenExpirationDate(string token);
        IEnumerable<Claim>? ParseClaimsFromJwt(string token);
    }
}
