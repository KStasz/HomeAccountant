using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.CategoriesService.Model
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(64)]
        public required string Name { get; set; }

        [Required]
        public required string CreatedBy { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
