using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class CategoryReadDtoToCategoryModelMapper : ITypeMapper<CategoryModel, CategoryReadDto>
    {
        public CategoryModel Map(CategoryReadDto? value)
        {
            value.Protect();

            return new CategoryModel()
            { 
                Id = value!.Id,
                Name = value.Name
            };
        }
    }
}
