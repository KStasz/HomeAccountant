using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.DTOs
{
    public class RegisterCreateDto
    {
        [Required(ErrorMessage = "Nazwa księgi jest wymagana")]
        public string? Name { get; set; }

        public void ClearModel()
        {
            Name = null;
        }
    }
}
