using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.BillingPeriod
{
    public record BillingPeriodCreateDto
    {
        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Name { get; set; }
    }
}
