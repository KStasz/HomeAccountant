using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Category
{
    public record CategoryCreateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Name { get; set; }
    }
}
