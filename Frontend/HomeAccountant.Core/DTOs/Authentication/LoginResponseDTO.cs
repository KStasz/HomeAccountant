namespace HomeAccountant.Core.DTOs.Authentication
{
    public record LoginResponseDto
    {
        public string? Token { get; init; }
        public string? RefreshToken { get; init; }
    }
}
