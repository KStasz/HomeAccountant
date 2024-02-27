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

            if (string.IsNullOrEmpty(value?.Token) || string.IsNullOrEmpty(value.RefreshToken))
                throw new ArgumentException();

            return new TokenAuthenticationModel(value!.Token, value.RefreshToken);
        }
    }
}
