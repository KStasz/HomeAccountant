using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class CategoryModelToCategoryCreateDtoMapper : ITypeMapper<CategoryCreateDto, CategoryModel>
    {
        public CategoryCreateDto Map(CategoryModel? value)
        {
            value.Protect();

            return new CategoryCreateDto()
            {
                Id = value!.Id,
                Name = value.Name,
            };
        }
    }
}
