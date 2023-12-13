using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.BillingPeriod
{
    public class BillingPeriodCreateDto : IClearableObject
    {
        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Name { get; set; }

        public void Clear()
        {
            Name = null;
        }
    }
}
