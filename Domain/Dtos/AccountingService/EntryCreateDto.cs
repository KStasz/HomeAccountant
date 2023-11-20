using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.AccountingService
{
    public class EntryCreateDto
    {
        [Required]
        public required string Name { get; set; }
        
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
