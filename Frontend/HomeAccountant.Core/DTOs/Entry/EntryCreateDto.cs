using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Entry
{
    public class EntryCreateDto : IClearableObject
    {
        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Name { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Kategoria jest wymagana")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(0.01d, double.MaxValue, ErrorMessage = "Cena jest wymagana")]
        public decimal Price { get; set; }

        public void Clear()
        {
            Name = null;
            CategoryId = 0;
            Price = 0;
        }
    }
}
