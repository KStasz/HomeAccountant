namespace HomeAccountant.Core.DTOs.Category
{
    public record CategoryReadDto
    {
        public int Id { get; init; }
        public string? Name { get; init; }
    }
}
