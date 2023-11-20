using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.IdentityPlatform
{
    public class UserPasswordChangeDto
    {
        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        public required string NewPassword { get; set; }
    }
}
