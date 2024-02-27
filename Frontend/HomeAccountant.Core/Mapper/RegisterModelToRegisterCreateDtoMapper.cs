using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class RegisterModelToRegisterCreateDtoMapper : ITypeMapper<RegisterCreateDto, RegisterModel>
    {
        public RegisterCreateDto Map(RegisterModel? value)
        {
            value.Protect();

            return new RegisterCreateDto()
            {
                Name = value!.Name,
                Description = value!.Description
            };
        }
    }
}
