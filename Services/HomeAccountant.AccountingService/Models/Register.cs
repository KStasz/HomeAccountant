using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.AccountingService.Models
{
    public class Register
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
        
        [Required]
        public required string CreatorId { get; set; }
        
        [Required]
        public bool IsActive { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; }
        
        public ICollection<Entry>? Entries { get; set; }
        public ICollection<RegisterSharing>? Sharings { get; set; }
    }
}
