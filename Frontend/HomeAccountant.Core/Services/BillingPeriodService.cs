using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Extensions;
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

        public BillingPeriodService(AuthorizableHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse> CloseBillingPeriodAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var xd = new HttpClient();
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Close";
                var response = await _httpClient.PutAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

                return responseContent.Protect();
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

        public async Task<ServiceResponse> CreateBillingPeriodAsync(int registerId, BillingPeriodCreateDto createDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod";
                var response = await _httpClient.PostAsJsonAsync(url, createDto, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

                return responseContent.Protect();
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

                return responseContent.Protect();
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

        public async Task<ServiceResponse<IEnumerable<BillingPeriodReadDto>>> GetBiilingPeriodsAsync(int registerId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content
                    .ReadFromJsonAsync<ServiceResponse<IEnumerable<BillingPeriodReadDto>>>(cancellationToken);

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<IEnumerable<BillingPeriodReadDto>>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse<BillingPeriodStatisticDto>> GetBillingPeriodStatisticAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Statistic";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<BillingPeriodStatisticDto>>();

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<BillingPeriodStatisticDto>(
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

                return responseContent.Protect();
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
