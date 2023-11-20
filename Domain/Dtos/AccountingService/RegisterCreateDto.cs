using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.AccountingService
{
    public class RegisterCreateDto
    {
        [Required]
        public required string Name { get; set; }
    }
}
