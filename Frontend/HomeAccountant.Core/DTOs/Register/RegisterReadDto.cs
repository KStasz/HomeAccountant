namespace HomeAccountant.Core.DTOs.Register
{
    public class RegisterReadDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RegisterReadDto dto &&
                   Id == dto.Id &&
                   Name == dto.Name &&
                   Description == dto.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
