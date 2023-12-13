using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.AccountingService.Models
{
    public class BillingPeriod
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
        
        [Required]
        public int RegisterId { get; set; }
        
        public bool IsOpen { get; set; } = true;

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public required Register Register { get; set; }
        public required ICollection<Entry> Entries { get; set; }
    }
}
