using HomeAccountant.Core.DTOs.Identity;

namespace HomeAccountant.Core.DTOs.Friends
{
    public record FriendshipReadDto
    {
        public int Id { get; init; }
        public UserModelReadDto? User { get; init; }
        public UserModelReadDto? Friend { get; init; }
        public bool IsAccepted { get; init; }
        public UserModelReadDto? CreatedBy { get; init; }
        public DateTime CreationDate { get; init; }
    }
}
