using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;
using Microsoft.Extensions.Logging;
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

        public async Task<ServiceResponse> CreateRegisterAsync(RegisterCreateDto registerCreateDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/Register", registerCreateDto, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

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

        public async Task<ServiceResponse<IEnumerable<RegisterReadDto>>> GetRegistersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "/api/Register";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                ServiceResponse<IEnumerable<RegisterReadDto>> ? responseContent = await response.Content
                    .ReadFromJsonAsync<ServiceResponse<IEnumerable<RegisterReadDto>>>(cancellationToken);

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

        public async Task<ServiceResponse> DeleteRegisterAsync(RegisterReadDto registerReadDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerReadDto.Id}";
                var response = await _httpClient.DeleteAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

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
