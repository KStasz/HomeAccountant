using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class LoginResponseDtoToLoginResponseModelMapper : ITypeMapper<LoginResponseModel, LoginResponseDto>
    {
        public LoginResponseModel Map(LoginResponseDto? value)
        {
            value.Protect();

            return new LoginResponseModel()
            {
                Token = value!.Token,
                RefreshToken = value.RefreshToken,
            };
        }
    }
}
