using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.FriendsService.Model
{
    [Index(nameof(UserId), nameof(FriendId), IsUnique = true)]
    public class Friend
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public required string FriendId { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
