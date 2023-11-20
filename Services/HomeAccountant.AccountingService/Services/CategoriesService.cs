
using Domain.Dtos.CategoryService;

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

        public async Task<CategoryReadDto?> GetCategory(int id)
        {
            var url = $"/api/Categories/{id}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) 
                return null;
            
            var result = await response.Content.ReadFromJsonAsync<CategoryReadDto?>();

            return result;
        }
    }
}
