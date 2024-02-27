using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Authentication
{
    public record RegisterUserDto
    {
        [EmailAddress(ErrorMessage = "Email jest niepoprawny")]
        [Required(ErrorMessage = "Email jest wymagany")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [Compare(nameof(RepeatedPassword), ErrorMessage = "Hasła nie są identyczne")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane")]
        public string? RepeatedPassword { get; set; }
    }
}
