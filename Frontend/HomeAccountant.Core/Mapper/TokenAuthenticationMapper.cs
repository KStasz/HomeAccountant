using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class TokenAuthenticationMapper : ITypeMapper<TokenAuthenticationModel, LoginResponseDTO>
    {
        public TokenAuthenticationModel Map(LoginResponseDTO value)
        {
            return new TokenAuthenticationModel(value.Token, value.RefreshToken);
        }
    }
}
