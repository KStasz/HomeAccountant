using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Core.Model
{
    public class RegisterModel : IClearableObject
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }

        public void Clear()
        {
            Name = null;
            Description = null;
        }

        public override bool Equals(object? obj)
        {
            return obj is RegisterModel dto &&
                   Id == dto.Id &&
                   Name == dto.Name &&
                   Description == dto.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
