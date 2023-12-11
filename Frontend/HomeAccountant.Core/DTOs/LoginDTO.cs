using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Hasło jest niepoprawne")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string? Password { get; set; }
    }
}
