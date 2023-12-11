using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Category
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
