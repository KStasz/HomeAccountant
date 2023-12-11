using HomeAccountant.Core.DTOs;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IEntryService
    {
        Task<ServiceResponse<IEnumerable<EntryReadDto>?>> GetEntries(int registerId);
        Task<ServiceResponse<EntryReadDto?>> CreateEntry(int registerId, EntryCreateDto entryCreateDto);
        Task<ServiceResponse> UpdateEntry(int registerId, EntryUpdateDto entryUpdateDto);
        Task<ServiceResponse> DeleteEntry(int registerId, int entryId);
    }
}
