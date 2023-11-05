using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.IdentityPlatform;

public class UserRegistrationRequestDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public required string UserName { get; set; }
    
    [Required]
    public required string Password { get; set; }
}
