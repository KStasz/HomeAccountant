using HomeAccountant.Core.DTOs.Identity;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class UserModelReadDtoToUserModelMapper : ITypeMapper<UserModel, UserModelReadDto>
    {
        public UserModel Map(UserModelReadDto? value)
        {
            value.Protect();

            return new UserModel()
            {
                Id = value!.Id,
                Email = value.Email,
                UserName = value.UserName
            };
        }
    }
}