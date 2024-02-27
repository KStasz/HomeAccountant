using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Authentication
{
    public record LoginDto
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Hasło jest niepoprawne")]
        public string? Email { get; init; }
        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string? Password { get; init; }
    }
}
