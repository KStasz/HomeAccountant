using AutoMapper;
using Domain.Dtos.CategoryService;
using HomeAccountant.CategoriesService.Model;

namespace HomeAccountant.CategoriesService.Profles
{
    public class CategoryServiceProfiles : Profile
    {
        public CategoryServiceProfiles()
        {
            CreateMap<CategoryModel, CategoryReadDto>();
            CreateMap<CategoryUpdateDto, CategoryModel>();
            CreateMap<CategoryCreateDto, CategoryModel>();
        }
    }
}
