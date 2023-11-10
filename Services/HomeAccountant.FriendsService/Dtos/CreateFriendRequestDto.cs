using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.FriendsService.Dtos
{
    public class CreateFriendRequestDto
    {
        [Required]
        public required string RecipientEmail { get; set; }
    }
}
