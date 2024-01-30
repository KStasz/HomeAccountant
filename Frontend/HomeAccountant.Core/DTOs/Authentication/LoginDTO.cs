using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Authentication
{
    public class LoginDTO : IClearableObject
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Hasło jest niepoprawne")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string? Password { get; set; }

        public void Clear()
        {
            Email = null;
            Password = null;
        }
    }
}
