using HomeAccountant.Core.DTOs;
using HomeAccountant.Core.Model;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly AuthorizableHttpClient _httpClient;
        private readonly ILogger<RegisterService> _logger;

        public RegisterService(AuthorizableHttpClient httpClient,
            ILogger<RegisterService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ServiceResponse> CreateRegisterAsync(RegisterCreateDto registerCreateDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Register", registerCreateDto);

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResponse(false);
                }

                return new ServiceResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new ServiceResponse(false);
            }
        }

        public async Task<ServiceResponse<IEnumerable<RegisterReadDto>>> GetRegistersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Register");

                if (!response.IsSuccessStatusCode)
                {
                    return new ServiceResponse<IEnumerable<RegisterReadDto>>(false);
                }

                var result = await response.Content.ReadFromJsonAsync<IEnumerable<RegisterReadDto>>();

                return new ServiceResponse<IEnumerable<RegisterReadDto>>(result ?? throw new ArgumentNullException(nameof(result)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new ServiceResponse<IEnumerable<RegisterReadDto>>(false);
            }

        }

        public async Task<ServiceResponse> DeleteRegisterAsync(RegisterReadDto registerReadDto)
        {
            var url = $"/api/Register/{registerReadDto.Id}";

            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse(false);
            }

            return new ServiceResponse(true);
        }
    }
}
