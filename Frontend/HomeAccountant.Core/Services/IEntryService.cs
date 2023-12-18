using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IEntryService
    {
        Task<ServiceResponse<PaggedResult<EntryReadDto>>> GetEntries(int registerId, int billingPeriodId, int page = 1, int recordsOnPage = 10);
        Task<ServiceResponse<EntryReadDto>> CreateEntry(int registerId, int billingPeriodId, EntryCreateDto entryCreateDto);
        Task<ServiceResponse> UpdateEntry(int registerId, int billingPeriodId, EntryUpdateDto entryUpdateDto);
        Task<ServiceResponse> DeleteEntry(int registerId, int billingPeriodId, int entryId);
    }
}
