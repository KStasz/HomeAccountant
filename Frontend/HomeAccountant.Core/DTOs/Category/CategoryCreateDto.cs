using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs.Category
{
    public class CategoryCreateDto : IClearableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana")]
        public string? Name { get; set; }

        public void Clear()
        {
            Id = 0;
            Name = null;
        }
    }
}
