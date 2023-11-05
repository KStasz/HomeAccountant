using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.IdentityPlatform;

public class TokenRequestDto
{
    [Required]
    public required string Token { get; set; }
    
    [Required]
    public required string RefreshToken { get; set; }
}
