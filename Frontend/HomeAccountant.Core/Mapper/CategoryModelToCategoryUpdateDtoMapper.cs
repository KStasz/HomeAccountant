using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class CategoryModelToCategoryUpdateDtoMapper : ITypeMapper<CategoryUpdateDto, CategoryModel>
    {
        public CategoryUpdateDto Map(CategoryModel? value)
        {
            value.Protect();

            return new CategoryUpdateDto()
            {
                Id = value!.Id,
                Name = value.Name,
            };
        }
    }
}
