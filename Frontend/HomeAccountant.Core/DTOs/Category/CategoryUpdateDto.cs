using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Category
{
    public record CategoryUpdateDto
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
