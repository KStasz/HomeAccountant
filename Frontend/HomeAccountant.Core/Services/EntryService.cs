using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;

namespace HomeAccountant.Core.Services
{
    public class EntryService : IEntryService
    {
        private readonly AuthorizableHttpClient _httpClient;

        public EntryService(AuthorizableHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ServiceResponse<EntryReadDto>> CreateEntry(int registerId, int billingPeriodId, EntryCreateDto entryCreateDto)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry";

                var response = await _httpClient.PostAsJsonAsync(url, entryCreateDto);

                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<EntryReadDto>>();

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EntryReadDto>(ex.Message);
            }
        }

        public async Task<ServiceResponse> DeleteEntry(int registerId, int billingPeriodId, int entryId)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry/{entryId}";

                var response = await _httpClient.DeleteAsync(url);

                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>();

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, new List<string>() { ex.Message });
            }
        }

        public async Task<ServiceResponse<PaggedResult<EntryReadDto>>> GetEntries(int registerId, int billingPeriodId, int page = 1, int recordsOnPage = 10)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry?page={page}&recordsOnPage={recordsOnPage}";

                var response = await _httpClient.GetAsync(url);

                var responseContent = await response.Content
                    .ReadFromJsonAsync<ServiceResponse<PaggedResult<EntryReadDto>>>();

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaggedResult<EntryReadDto>>(ex.Message);
            }
        }

        public async Task<ServiceResponse> UpdateEntry(int registerId, int billingPeriodId, EntryUpdateDto entryUpdateDto)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry";

                var response = await _httpClient.PutAsJsonAsync(url, entryUpdateDto);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>();

                return responseContent.Protect();
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, new List<string>() { ex.Message });
            }
        }
    }
}
