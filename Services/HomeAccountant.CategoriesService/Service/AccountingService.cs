
using Domain.Dtos.AccountingService;
using Domain.Model;

namespace HomeAccountant.CategoriesService.Service
{
    public class AccountingService : IAccountingService
    {
        private readonly AuthorizedHttpClient _httpClient;
        private readonly ILogger<AccountingService> _logger;

        public AccountingService(AuthorizedHttpClient httpClient,
            ILogger<AccountingService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public Task DeleteEntriesByCategoryId(int categoryId)
        {
            var url = $"/api/Register/Entries/{categoryId}";

            return _httpClient.DeleteAsync(url);
        }

        public async Task<ServiceResponse<RegisterReadDto?>> GetRegister(int registerId)
        {
            try
            {
                var url = $"/api/Register/{registerId}";
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<RegisterReadDto?>>();

                if (responseContent is null)
                    return new ServiceResponse<RegisterReadDto?>(
                        new string[]
                        {
                            "Reading register failed"
                        });

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return new ServiceResponse<RegisterReadDto?>(
                    new string[]
                    {
                        "Reading register failed"
                    });
            }
        }
    }
}
