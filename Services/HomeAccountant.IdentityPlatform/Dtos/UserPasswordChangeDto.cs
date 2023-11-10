using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.IdentityPlatform.Dtos
{
    public class UserPasswordChangeDto
    {
        [Required]
        public required string CurrentPassword { get; set; }
        
        [Required]
        public required string NewPassword { get; set; }
    }
}
