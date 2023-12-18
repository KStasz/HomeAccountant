using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

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
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>();

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new ServiceResponse<ServiceResponse>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse<IEnumerable<RegisterReadDto>>> GetRegistersAsync()
        {
            try
            {
                var url = "/api/Register";
                var response = await _httpClient.GetAsync(url);
                ServiceResponse<IEnumerable<RegisterReadDto>> ? responseContent = await response.Content
                    .ReadFromJsonAsync<ServiceResponse<IEnumerable<RegisterReadDto>>>();

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new ServiceResponse<IEnumerable<RegisterReadDto>>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse> DeleteRegisterAsync(RegisterReadDto registerReadDto)
        {
            try
            {
                var url = $"/api/Register/{registerReadDto.Id}";
                var response = await _httpClient.DeleteAsync(url);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>();

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new ServiceResponse(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }
    }
}
