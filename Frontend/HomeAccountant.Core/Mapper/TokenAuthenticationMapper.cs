using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class TokenAuthenticationMapper : ITypeMapper<TokenAuthenticationModel, LoginResponseDto>
    {
        public TokenAuthenticationModel Map(LoginResponseDto? value)
        {
            value.Protect();

            return new TokenAuthenticationModel(value!.Token, value.RefreshToken);
        }
    }
}
