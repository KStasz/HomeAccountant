using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Register
{
    public record RegisterCreateDto
    {
        [Required(ErrorMessage = "Nazwa księgi jest wymagana")]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
