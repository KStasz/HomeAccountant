using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Entry
{
    public record EntryUpdateDto
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
