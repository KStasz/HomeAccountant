using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.AccountingService
{
    public class BillingPeriodUpdateDto
    {
        [Required]
        public required string Name { get; set; }
        public bool IsOpen { get; set; }
    }
}
