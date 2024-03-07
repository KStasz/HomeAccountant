using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Exceptions;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly AuthorizableHttpClient _httpClient;
        private readonly ITypeMapper<CategoryCreateDto, CategoryModel> _categoryCreateMapper;
        private readonly ITypeMapper<CategoryUpdateDto, CategoryModel> _categoryUpdateMapper;
        private readonly ITypeMapper<CategoryModel, CategoryReadDto> _categoryMapper;
        private readonly ILogger<CategoriesService> _logger;

        public CategoriesService(AuthorizableHttpClient httpClient,
            ITypeMapper<CategoryCreateDto, CategoryModel> categoryCreateMapper,
            ITypeMapper<CategoryUpdateDto, CategoryModel> categoryUpdateMapper,
            ITypeMapper<CategoryModel, CategoryReadDto> categoryMapper,
            ILogger<CategoriesService> logger)
        {
            _httpClient = httpClient;
            _categoryCreateMapper = categoryCreateMapper;
            _categoryUpdateMapper = categoryUpdateMapper;
            _categoryMapper = categoryMapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<CategoryModel?>> CreateCategoryAsync(CategoryModel categoryModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "/api/Categories";
                var model = _categoryCreateMapper.Map(categoryModel);
                var response = await _httpClient.PostAsJsonAsync(url, model, cancellationToken);

                var categoryResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<CategoryReadDto>>(cancellationToken);

                if (categoryResponse is null)
                    throw new ServiceException();

                if (!categoryResponse.Result)
                    return new ServiceResponse<CategoryModel?>(
                        categoryResponse.Result,
                        categoryResponse.Errors);

                return new ServiceResponse<CategoryModel?>(_categoryMapper.Map(categoryResponse.Value));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CategoryModel?>(
                    false,
                    new List<string>
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var url = $"/api/Categories/{categoryId}";

            var response = await _httpClient.DeleteAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return new ServiceResponse(false);

            return new ServiceResponse(true);
        }

        public async Task<ServiceResponse<IEnumerable<CategoryModel>?>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "/api/Categories";

                var response = await _httpClient.GetAsync(url, cancellationToken);
                var categoryResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<IEnumerable<CategoryReadDto>>>(cancellationToken);

                if (categoryResponse is null)
                    throw new ServiceException();

                if (!categoryResponse.Result)
                    return new ServiceResponse<IEnumerable<CategoryModel>?>(
                        categoryResponse.Result,
                        categoryResponse?.Errors);

                return new ServiceResponse<IEnumerable<CategoryModel>?>(
                    categoryResponse.Value?.Select(_categoryMapper.Map));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<CategoryModel>?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse<IEnumerable<CategoryModel>?>> GetRegisterCategories(int registerId, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"/api/Categories/RegisterCategories?registerId={registerId}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<IEnumerable<CategoryReadDto>?>>();

                if (responseContent is null)
                    return new ServiceResponse<IEnumerable<CategoryModel>?>(
                        false,
                        ["Reading categories failed"]);

                if (!responseContent.Result)
                    return new ServiceResponse<IEnumerable<CategoryModel>?>(
                        responseContent.Result,
                        responseContent?.Errors);

                return new ServiceResponse<IEnumerable<CategoryModel>?>(
                    responseContent.Value?.Select(_categoryMapper.Map));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return new ServiceResponse<IEnumerable<CategoryModel>?>(
                    false,
                    ["Reading categories failed"]);
            }
        }

        public async Task<ServiceResponse> UpdateCategoryAsync(CategoryModel categoryModel, CancellationToken cancellationToken = default)
        {
            var url = "/api/Categories";
            var model = _categoryUpdateMapper.Map(categoryModel);
            var response = await _httpClient.PutAsJsonAsync(url, model, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return new ServiceResponse(false);

            return new ServiceResponse(true);
        }
    }
}
