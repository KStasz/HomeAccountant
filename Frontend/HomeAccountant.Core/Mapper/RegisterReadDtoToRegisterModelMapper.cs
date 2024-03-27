using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class RegisterReadDtoToRegisterModelMapper : ITypeMapper<RegisterModel, RegisterReadDto>
    {
        public RegisterModel Map(RegisterReadDto? value)
        {
            value.Protect();

            return new RegisterModel()
            {
                Id = value!.Id,
                Name = value.Name,
                Description = value.Description,
                CreatedDate = value.CreatedDate,
                UserIds = value.UserIds
            };
        }
    }
}
