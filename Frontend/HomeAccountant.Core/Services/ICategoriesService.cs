using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface ICategoriesService
    {
        Task<ServiceResponse<IEnumerable<CategoryReadDto>?>> GetCategoriesAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<CategoryReadDto?>> CreateCategoryAsync(CategoryCreateDto categoryCreateDto, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<ServiceResponse> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto, CancellationToken cancellationToken = default);
    }
}
