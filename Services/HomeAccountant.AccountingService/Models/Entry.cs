using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.AccountingService.Models
{
    public class Entry
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
        
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BillingPeriodId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public required string CreatedBy { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public required BillingPeriod BillingPeriod { get; set; }
    }
}
