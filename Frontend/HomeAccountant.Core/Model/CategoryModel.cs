using HomeAccountant.Core.DTOs;
using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.Model
{
    public class CategoryModel : IClearableObject
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
