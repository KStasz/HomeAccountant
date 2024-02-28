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

        public override bool Equals(object? obj)
        {
            return obj is CategoryModel model &&
                   Id == model.Id &&
                   Name == model.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
