using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IEntryService
    {
        Task<ServiceResponse<PaggedResult<EntryReadDto>>> GetEntriesAsync(int registerId, int billingPeriodId, int page = 1, int recordsOnPage = 10, CancellationToken cancellationToken = default);
        Task<ServiceResponse<EntryReadDto>> CreateEntryAsync(int registerId, int billingPeriodId, EntryCreateDto entryCreateDto, CancellationToken cancellationToken = default);
        Task<ServiceResponse> UpdateEntryAsync(int registerId, int billingPeriodId, EntryUpdateDto entryUpdateDto, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteEntryAsync(int registerId, int billingPeriodId, int entryId, CancellationToken cancellationToken = default);
    }
}
