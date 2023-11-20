using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.IdentityPlatform;

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
