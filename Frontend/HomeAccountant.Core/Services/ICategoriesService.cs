using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface ICategoriesService
    {
        Task<ServiceResponse<IEnumerable<CategoryModel>?>> GetCategoriesAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse<CategoryModel?>> CreateCategoryAsync(CategoryModel categoryModel, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
        Task<ServiceResponse> UpdateCategoryAsync(CategoryModel categoryModel, CancellationToken cancellationToken = default);
    }
}
