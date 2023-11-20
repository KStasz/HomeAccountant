using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.IdentityPlatform;

public class TokenRequestDto
{
    [Required]
    public required string Token { get; set; }

    [Required]
    public required string RefreshToken { get; set; }
}
