using Domain.Dtos.IdentityPlatform;

namespace Domain.Dtos.FriendsService
{
    public record FriendshipReadDto
    {
        public int Id { get; init; }
        public UserModelDto? User { get; init; }
        public UserModelDto? Friend { get; init; }
        public bool IsAccepted { get; init; }
        public UserModelDto? CreatedBy { get; init; }
        public DateTime CreationDate { get; init; }
    }
}
