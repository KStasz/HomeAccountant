using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.FriendsService
{
    public class CreateFriendRequestDto
    {
        [Required]
        public required string RecipientEmail { get; set; }
    }
}
