using Domain.Dtos.CategoryService;
using Domain.Model;

namespace HomeAccountant.AccountingService.Services
{
    public interface ICategoriesService
    {
        Task<bool> CategoryExists(int id);
        Task<ServiceResponse<CategoryReadDto?>> GetCategoryAsync(int id);
    }
}
