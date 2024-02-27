using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Exceptions;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using System.Net.Http.Json;

namespace HomeAccountant.Core.Services
{
    public class EntryService : IEntryService
    {
        private readonly AuthorizableHttpClient _httpClient;
        private readonly ITypeMapper<EntryModel, EntryReadDto> _entryMapper;
        private readonly ITypeMapper<EntryCreateDto, EntryModel> _entryCreateMapper;
        private readonly ITypeMapper<EntryUpdateDto, EntryModel> _entryUpdateMapper;

        public EntryService(AuthorizableHttpClient httpClient,
            ITypeMapper<EntryModel, EntryReadDto> entryMapper,
            ITypeMapper<EntryCreateDto, EntryModel> entryCreateMapper,
            ITypeMapper<EntryUpdateDto, EntryModel> entryUpdateMapper)
        {
            _httpClient = httpClient;
            _entryMapper = entryMapper;
            _entryCreateMapper = entryCreateMapper;
            _entryUpdateMapper = entryUpdateMapper;
        }

        public async Task<ServiceResponse<EntryModel?>> CreateEntryAsync(int registerId, int billingPeriodId, EntryModel entryModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry";
                var model = _entryCreateMapper.Map(entryModel);
                var response = await _httpClient.PostAsJsonAsync(url, model, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse<EntryReadDto>>(cancellationToken);

                if (responseContent is null)
                    throw new ServiceException();

                if (!responseContent.Result)
                    return new ServiceResponse<EntryModel?>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse<EntryModel?>(_entryMapper.Map(responseContent.Value));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<EntryModel?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    }
                    );
            }
        }

        public async Task<ServiceResponse> DeleteEntryAsync(int registerId, int billingPeriodId, int entryId, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry/{entryId}";
                var response = await _httpClient.DeleteAsync(url, cancellationToken);
                var responseContent = await response.Content.ReadFromJsonAsync<ServiceResponse>(cancellationToken);

                if (responseContent is null)
                    throw new ServiceException();

                if (!responseContent.Result)
                    return new ServiceResponse<EntryModel>(
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

        public async Task<ServiceResponse<PaggedResult<EntryModel>?>> GetEntriesAsync(int registerId, int billingPeriodId, int page = 1, int recordsOnPage = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry?page={page}&recordsOnPage={recordsOnPage}";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                var responseContent = await response.Content
                    .ReadFromJsonAsync<ServiceResponse<PaggedResult<EntryReadDto>>>(cancellationToken);

                if (responseContent is null)
                    throw new ServiceException();

                if (!responseContent.Result)
                    return new ServiceResponse<PaggedResult<EntryModel>?>(
                        responseContent.Result,
                        responseContent.Errors);

                return new ServiceResponse<PaggedResult<EntryModel>?>(
                    new PaggedResult<EntryModel>(
                        responseContent.Value?.Result?.Select(_entryMapper.Map),
                        responseContent.Value?.CurrentPage ?? 0,
                        responseContent.Value?.TotalPages ?? 0));
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaggedResult<EntryModel>?>(
                    false,
                    new List<string>()
                    {
                        ex.Message
                    });
            }
        }

        public async Task<ServiceResponse> UpdateEntryAsync(int registerId, int billingPeriodId, EntryModel entryModel, CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"/api/Register/{registerId}/BillingPeriod/{billingPeriodId}/Entry";
                var model = _entryUpdateMapper.Map(entryModel);
                var response = await _httpClient.PutAsJsonAsync(url, model, cancellationToken);
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
