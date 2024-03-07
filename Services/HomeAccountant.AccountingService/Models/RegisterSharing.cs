using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.AccountingService.Models
{
    public class RegisterSharing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RegisterId { get; set; }

        [Required]
        public required string OwnerId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public Register? Register { get; set; }
    }
}
