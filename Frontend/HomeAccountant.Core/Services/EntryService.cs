using HomeAccountant.Core.DTOs;
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

        public async Task<ServiceResponse<EntryReadDto?>> CreateEntry(int registerId, EntryCreateDto entryCreateDto)
        {
            var url = $"/api/Register/{registerId}/Entry";

            var response = await _httpClient.PostAsJsonAsync(url, entryCreateDto);

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse<EntryReadDto?>(false);
            }

            var result = await response.Content.ReadFromJsonAsync<EntryReadDto>();

            return new ServiceResponse<EntryReadDto?>(result);
        }

        public async Task<ServiceResponse> DeleteEntry(int registerId, int entryId)
        {
            var url = $"/api/Register/{registerId}/Entry/{entryId}";

            var response = await _httpClient.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
                return new ServiceResponse(false);

            return new ServiceResponse(true);
        }

        public async Task<ServiceResponse<IEnumerable<EntryReadDto>?>> GetEntries(int registerId)
        {
            var url = $"/api/Register/{registerId}/Entry";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse<IEnumerable<EntryReadDto>?>(false);
            }

            var result = await response.Content.ReadFromJsonAsync<IEnumerable<EntryReadDto>>();

            return new ServiceResponse<IEnumerable<EntryReadDto>?>(result);
        }

        public async Task<ServiceResponse> UpdateEntry(int registerId, EntryUpdateDto entryUpdateDto)
        {
            var url = $"/api/Register/{registerId}/Entry";

            var response = await _httpClient.PutAsJsonAsync(url, entryUpdateDto);

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResponse(false);
            }

            return new ServiceResponse(true);
        }




    }
}
