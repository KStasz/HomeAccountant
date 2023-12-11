using Domain.Dtos.AccountingService;

namespace HomeAccountant.CategoriesService.Service
{
    public interface IAccountingService
    {
        Task DeleteEntriesByCategoryId(int categoryId);
    }
}
