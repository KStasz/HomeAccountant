using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.FriendsService.Model
{
    public class FriendRequest
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public required string CreatorId { get; set; }

        [Required]
        public required string RecipientId { get; set; }

        [Required]
        public bool IsRejected { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
