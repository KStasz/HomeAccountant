namespace HomeAccountant.Core.DTOs.Register
{
    public record ShareRegisterCreateDto
    {
        public int RegisterId { get; init; }
        public string? UserId { get; init; }
    }
}
