using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JwtAuthenticationManager.Model;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string UserId { get; set; }

    [Required]
    public required string Token { get; set; }

    [Required]
    public required string JwtId { get; set; }

    [Required]
    public bool IsUsed { get; set; }

    [Required]
    public bool IsRevoked { get; set; }

    [Required]
    public DateTime AddedDate { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
