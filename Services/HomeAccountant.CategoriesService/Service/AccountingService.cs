
using Domain.Dtos.AccountingService;

namespace HomeAccountant.CategoriesService.Service
{
    public class AccountingService : IAccountingService
    {
        private readonly AuthorizedHttpClient _httpClient;

        public AccountingService(AuthorizedHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task DeleteEntriesByCategoryId(int categoryId)
        {
            var url = $"/api/Register/Entries/{categoryId}";

            return _httpClient.DeleteAsync(url);
        }
    }
}
