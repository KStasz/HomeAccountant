using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.FriendsService.Dtos
{
    public class CreateFriendRequestDto
    {
        [Required]
        public required string CreatorId { get; set; }

        [Required]
        public required string RecipientId { get; set; }
    }
}
