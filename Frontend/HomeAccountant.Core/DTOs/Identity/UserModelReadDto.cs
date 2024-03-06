namespace HomeAccountant.Core.DTOs.Identity
{
    public record UserModelReadDto
    {
        public string? Id { get; init; }
        public string? Email { get; init; }
        public string? UserName { get; init; }
    }
}
