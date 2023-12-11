using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Model;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly AuthorizableHttpClient _httpClient;

        public CategoriesService(AuthorizableHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<CategoryReadDto?>> CreateCategoryAsync(CategoryCreateDto categoryCreateDto)
        {
            var url = "/api/Categories";

            var response = await _httpClient.PostAsJsonAsync(url, categoryCreateDto);

            if (!response.IsSuccessStatusCode)
                return new ServiceResponse<CategoryReadDto?>(false);

            var category = await response.Content.ReadFromJsonAsync<CategoryReadDto>();

            return new ServiceResponse<CategoryReadDto?>(category);
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(int categoryId)
        {
            var url = $"/api/Categories/{categoryId}";

            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ServiceResponse(false);

            return new ServiceResponse(true);
        }

        public async Task<ServiceResponse<IEnumerable<CategoryReadDto>?>> GetCategoriesAsync()
        {
            var url = "/api/Categories";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ServiceResponse<IEnumerable<CategoryReadDto>?>(false);

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<CategoryReadDto>>();

            return new ServiceResponse<IEnumerable<CategoryReadDto>?>(result);
        }

        public async Task<ServiceResponse> UpdateCategoryAsync(CategoryUpdateDto categoryUpdateDto)
        {
            var url = "/api/Categories";

            var response = await _httpClient.PutAsJsonAsync(url, categoryUpdateDto);

            if (!response.IsSuccessStatusCode)
                return new ServiceResponse(false);

            return new ServiceResponse(true);
        }
    }
}
