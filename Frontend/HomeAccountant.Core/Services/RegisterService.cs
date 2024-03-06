using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Exceptions;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly AuthorizableHttpClient _httpClient;
        private readonly ITypeMapper<RegisterCreateDto, RegisterModel> _registerCreateMapper;
        private readonly ITypeMapper<RegisterModel, RegisterReadDto> _registerMapper;
        private readonly ILogger<RegisterService> _logger;

        public RegisterService(AuthorizableHttpClient httpClient,
            ITypeMapper<RegisterCreateDto, RegisterModel> registerCreateMapper,
            ITypeMapper<RegisterModel, RegisterReadDto> registerMapper,
            ILogger<RegisterService> logger)
        {
            _httpClient = httpClient;
            _registerCreateMapper = registerCreateMapper;
            _registerMapper = registerMapper;
            _logger = logger;
        }

        public async Task<ServiceResponse> CreateRegisterAsync(RegisterModel registerModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "/api/Register";
                var model = _registerCreateMapper.Map(registerModel);
                var response = await _httpClient.PostAsJsonAsync(url, model, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

                if (responseContent is null)
                    throw new ServiceException();

                if (!responseContent.Result)
                    return new ServiceResponse(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse(true);
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

        public async Task<ServiceResponse<IEnumerable<RegisterModel>?>> GetRegistersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var url = "/api/Register";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<IEnumerable<RegisterReadDto>?>>(cancellationToken);

                if (responseContent is null)
                    throw new ServiceException();

                if (!responseContent.Result)
                    return new ServiceResponse<IEnumerable<RegisterModel>?>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse<IEnumerable<RegisterModel>?>(
                    responseContent.Value?.Select(_registerMapper.Map));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return new ServiceResponse<IEnumerable<RegisterModel>?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse> DeleteRegisterAsync(int registerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}";
                var response = await _httpClient.DeleteAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

                if (responseContent is null)
                    throw new ServiceException();

                if (!responseContent.Result)
                    return new ServiceResponse<ServiceResponse>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse(true);
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

        public async Task<ServiceResponse<RegisterModel?>> GetRegister(int registerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<RegisterReadDto?>>();

                if (responseContent is null)
                    return new ServiceResponse<RegisterModel?>(
                        false,
                        ["Unable to read specific register"]);

                if (!responseContent.Result)
                    return new ServiceResponse<RegisterModel?>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse<RegisterModel?>(_registerMapper.Map(responseContent.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");

                return new ServiceResponse<RegisterModel?>(
                    false,
                    ["Unable to read specific register"]);
            }
        }
    }
}
