using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.FriendsService.Model
{
    [Index(nameof(UserId), nameof(FriendId), IsUnique = true)]
    [Index(nameof(FriendId), nameof(UserId), IsUnique = true)]
    public class Friendships
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string UserId { get; set; }

        [Required]
        public required string FriendId { get; set; }

        [Required]
        public bool IsAccepted { get; set; } = false;

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public required string CreatorId { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
