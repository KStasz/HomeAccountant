namespace HomeAccountant.Core.DTOs.Category
{
    public record CategoryReadDto
    {
        public int Id { get; init; }
        public required string Name { get; init; }
    }
}
