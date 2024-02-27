namespace HomeAccountant.Core.Model
{
    public class BillingPeriodModel : IClearableObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public RegisterModel? Register { get; set; }
        public bool IsOpen { get; set; }
        public DateTime CreationDate { get; set; }
        public IEnumerable<EntryModel?>? Entries { get; set; }

        public void Clear()
        {
            Name = null;
        }

        public override bool Equals(object? obj)
        {
            return obj is BillingPeriodModel dto0 &&
                   Id == dto0.Id &&
                   Name == dto0.Name &&
                   (Register?.Equals(dto0.Register) ?? false) &&
                   IsOpen == dto0.IsOpen &&
                   CreationDate == dto0.CreationDate &&
                   (Entries is null ? Array.Empty<EntryModel>() : Entries).SequenceEqual(dto0.Entries is null ? Array.Empty<EntryModel>() : dto0.Entries);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Register, IsOpen, CreationDate, Entries);
        }
    }
}
