using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.IdentityPlatform;

public class UserLoginRequestDto
{
    [EmailAddress]
    [Required]
    public required string Email { get; set; }
    
    [Required]
    public required string Password { get; set; }
}
