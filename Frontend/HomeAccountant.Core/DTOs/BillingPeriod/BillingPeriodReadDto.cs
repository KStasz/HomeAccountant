using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.DTOs.Register;

namespace HomeAccountant.Core.DTOs.BillingPeriod
{
    public record BillingPeriodReadDto
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public RegisterReadDto? Register { get; init; }
        public bool IsOpen { get; init; }
        public DateTime CreationDate { get; init; }
        public IEnumerable<EntryReadDto>? Entries { get; init; }
    }
}
