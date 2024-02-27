namespace HomeAccountant.Core.DTOs.Register
{
    public record RegisterReadDto
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public DateTime CreatedDate { get; init; }
    }
}
