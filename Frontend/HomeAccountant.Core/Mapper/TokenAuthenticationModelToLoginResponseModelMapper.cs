using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class TokenAuthenticationModelToLoginResponseModelMapper : ITypeMapper<LoginResponseModel, TokenAuthenticationModel>
    {
        public LoginResponseModel Map(TokenAuthenticationModel? value)
        {
            value.Protect();

            return new LoginResponseModel()
            {
                Token = value?.Token,
                RefreshToken = value?.RefreshToken
            };
        }
    }
}
