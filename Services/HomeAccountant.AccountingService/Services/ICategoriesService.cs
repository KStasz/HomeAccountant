using Domain.Dtos.CategoryService;

namespace HomeAccountant.AccountingService.Services
{
    public interface ICategoriesService
    {
        Task<bool> CategoryExists(int id);
        Task<CategoryReadDto?> GetCategoryAsync(int id);
    }
}
