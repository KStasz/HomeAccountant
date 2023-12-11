using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs
{
    public class EntryUpdateDto
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
