using Domain.Dtos.AccountingService;
using Domain.Model;

namespace HomeAccountant.CategoriesService.Service
{
    public interface IAccountingService
    {
        Task DeleteEntriesByCategoryId(int categoryId);
        Task<ServiceResponse<RegisterReadDto?>> GetRegister(int registerId);
    }
}
