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

        public override bool Equals(object? obj)
        {

            return obj is BillingPeriodReadDto dto0 &&
                   Id == dto0.Id &&
                   Name == dto0.Name &&
                   (Register?.Equals(dto0.Register) ?? false) &&
                   IsOpen == dto0.IsOpen &&
                   CreationDate == dto0.CreationDate &&
                   (Entries is null ? new EntryReadDto[0] : Entries).SequenceEqual(dto0.Entries is null ? new EntryReadDto[0] : dto0.Entries);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Register, IsOpen, CreationDate, Entries);
        }
    }
}
