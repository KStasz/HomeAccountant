using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.IdentityPlatform;

public class UserLoginRequestDto
{
    [EmailAddress]
    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
}
