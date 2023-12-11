using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface ICategoriesService
    {
        Task<ServiceResponse<IEnumerable<CategoryReadDto>?>> GetCategoriesAsync();
        Task<ServiceResponse<CategoryReadDto?>> CreateCategoryAsync(CategoryCreateDto categoryCreateDto);
        Task<ServiceResponse> DeleteCategoryAsync(int categoryId);
        Task<ServiceResponse> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto);
    }
}
