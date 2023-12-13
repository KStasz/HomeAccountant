using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IEntryService
    {
        Task<ServiceResponse<IEnumerable<EntryReadDto>>> GetEntries(int registerId, int billingPeriodId);
        Task<ServiceResponse<EntryReadDto>> CreateEntry(int registerId, int billingPeriodId, EntryCreateDto entryCreateDto);
        Task<ServiceResponse> UpdateEntry(int registerId, int billingPeriodId, EntryUpdateDto entryUpdateDto);
        Task<ServiceResponse> DeleteEntry(int registerId, int billingPeriodId, int entryId);
    }
}
