using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Exceptions;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using System.Drawing;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HomeAccountant.Core.Services
{
    public class BillingPeriodService : IBillingPeriodService
    {
        private readonly AuthorizableHttpClient _httpClient;
        private readonly ITypeMapper<BillingPeriodCreateDto, BillingPeriodModel> _mapper;
        private readonly ITypeMapper<BillingPeriodModel, BillingPeriodReadDto> _billingPeriodMapper;
        private readonly ITypeMapper<BillingPeriodStatisticModel, BillingPeriodStatisticDto> _billingPeriodStatisticMapper;

        public BillingPeriodService(AuthorizableHttpClient httpClient,
            ITypeMapper<BillingPeriodCreateDto, BillingPeriodModel> mapper,
            ITypeMapper<BillingPeriodModel, BillingPeriodReadDto> billingPeriodMapper,
            ITypeMapper<BillingPeriodStatisticModel, BillingPeriodStatisticDto> billingPeriodStatisticMapper)
        {
            _httpClient = httpClient;
            _mapper = mapper;
            _billingPeriodMapper = billingPeriodMapper;
            _billingPeriodStatisticMapper = billingPeriodStatisticMapper;
        }

        public async Task<ServiceResponse> CloseBillingPeriodAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var xd = new HttpClient();
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Close";
                var response = await _httpClient.PutAsync(url, cancellationToken);
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
                return new ServiceResponse(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse> CreateBillingPeriodAsync(int registerId, BillingPeriodModel billingPeriod, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod";
                var model = _mapper.Map(billingPeriod);
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
                return new ServiceResponse(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse> DeleteBillingPeriodAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}";
                var response = await _httpClient.DeleteAsync(url, cancellationToken);
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
                return new ServiceResponse(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }

        }

        public async Task<ServiceResponse<IEnumerable<BillingPeriodModel>?>> GetBiilingPeriodsAsync(int registerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content
                    .ReadFromJsonAsync<ServiceResponse<IEnumerable<BillingPeriodReadDto>>>(cancellationToken);

                if (responseContent is null)
                    throw new ServiceException();

                if (!responseContent.Result)
                    return new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    responseContent.Value?.Select(_billingPeriodMapper.Map));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<BillingPeriodModel>?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse<BillingPeriodStatisticModel?>> GetBillingPeriodStatisticAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Statistic";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<BillingPeriodStatisticDto>>();

                if (responseContent is null)
                    throw new ServiceException();

                if (!responseContent.Result)
                    return new ServiceResponse<BillingPeriodStatisticModel?>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse<BillingPeriodStatisticModel?>(
                    _billingPeriodStatisticMapper.Map(responseContent.Value));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BillingPeriodStatisticModel?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse> OpenBillingPeriodAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Open";
                var response = await _httpClient.PutAsync(url, cancellationToken);
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
