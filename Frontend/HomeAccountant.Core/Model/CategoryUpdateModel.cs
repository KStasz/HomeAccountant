using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.Model
{
    public class CategoryUpdateModel
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
