using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.AccountingService
{
    public class BillingPeriodCreateDto
    {
        [Required]
        public required string Name { get; set; }
    }
}
