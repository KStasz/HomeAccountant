using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class EntryService : IEntryService
    {
        private readonly AuthorizableHttpClient _httpClient;

        public EntryService(AuthorizableHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<EntryReadDto>> CreateEntryAsync(int registerId, int billingPeriodId, EntryCreateDto entryCreateDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry";

                var response = await _httpClient.PostAsJsonAsync(url, entryCreateDto, cancellationToken);

                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<EntryReadDto>>(cancellationToken);

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EntryReadDto>(ex.Message);
            }
        }

        public async Task<ServiceResponse> DeleteEntryAsync(int registerId, int billingPeriodId, int entryId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry/{entryId}";

                var response = await _httpClient.DeleteAsync(url, cancellationToken);

                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, new List<string>() { ex.Message });
            }
        }

        public async Task<ServiceResponse<PaggedResult<EntryReadDto>>> GetEntriesAsync(int registerId, int billingPeriodId, int page = 1, int recordsOnPage = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry?page={page}&recordsOnPage={recordsOnPage}";

                var response = await _httpClient.GetAsync(url, cancellationToken);

                var responseContent = await response.Content
                    .ReadFromJsonAsync<ServiceResponse<PaggedResult<EntryReadDto>>>(cancellationToken);

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaggedResult<EntryReadDto>>(ex.Message);
            }
        }

        public async Task<ServiceResponse> UpdateEntryAsync(int registerId, int billingPeriodId, EntryUpdateDto entryUpdateDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry";

                var response = await _httpClient.PutAsJsonAsync(url, entryUpdateDto, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, new List<string>() { ex.Message });
            }
        }
    }
}
