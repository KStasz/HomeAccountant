using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.DTOs.Register;

namespace HomeAccountant.Core.DTOs.BillingPeriod
{
    public class BillingPeriodReadDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public RegisterReadDto? Register { get; set; }
        public bool IsOpen { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<EntryReadDto>? Entries { get; set; }
    }
}
