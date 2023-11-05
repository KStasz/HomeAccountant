using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.IdentityPlatform;

public class Team
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required string Name { get; set; }

    public bool IsActive { get; set; } = true;
    
    public DateTime CreationDate { get; set; } = DateTime.Now;
}
