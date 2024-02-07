using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class TokenAuthenticationMapper : ITypeMapper<TokenAuthenticationModel, LoginResponseDTO>
    {
        public TokenAuthenticationModel? Map(LoginResponseDTO? value)
        {
            if (value == null)
                return null;

            return new TokenAuthenticationModel(value.Token, value.RefreshToken);
        }
    }
}
