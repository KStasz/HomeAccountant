using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.Model
{
    public class RegisterUserModel : IClearableObject
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

        public void Clear()
        {
            Email = null;
            UserName = null;
            Password = null;
            RepeatedPassword = null;
        }
    }
}
