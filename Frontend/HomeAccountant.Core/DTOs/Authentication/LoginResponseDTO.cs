namespace HomeAccountant.Core.DTOs.Authentication
{
    public record LoginResponseDto
    {
        public required string Token { get; init; }
        public required string RefreshToken { get; init; }
    }
}
