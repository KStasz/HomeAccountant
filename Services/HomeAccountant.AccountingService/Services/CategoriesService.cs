
using Domain.Dtos.CategoryService;
using Domain.Model;

namespace HomeAccountant.AccountingService.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly HttpClient _httpClient;

        public CategoriesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CategoryExists(int id)
        {
            var url = $"/api/Categories/{id}";

            var response = await _httpClient.GetAsync(url);

            return response.IsSuccessStatusCode;
        }

        public async Task<ServiceResponse<CategoryReadDto?>> GetCategoryAsync(int id)
        {
            try
            {
                var url = $"/api/Categories/{id}";
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<CategoryReadDto?>>();

                if (responseContent is null)
                    throw new Exception("Unable to read category");

                if (!responseContent.Result)
                    return new ServiceResponse<CategoryReadDto?>()
                    {
                        Result = responseContent.Result,
                        Errors = responseContent.Errors
                    };

                return new ServiceResponse<CategoryReadDto?>(responseContent.Value);
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CategoryReadDto?>()
                {
                    Result = false,
                    Errors = new List<string>()
                    {
                        ex.Message
                    }
                };
            }


        }
    }
}
