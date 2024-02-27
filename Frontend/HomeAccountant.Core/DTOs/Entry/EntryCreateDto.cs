using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Entry
{
    public record EntryCreateDto
    {
        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Name { get; init; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Kategoria jest wymagana")]
        public int CategoryId { get; init; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(0.01d, double.MaxValue, ErrorMessage = "Cena jest wymagana")]
        public decimal Price { get; init; }
    }
}
