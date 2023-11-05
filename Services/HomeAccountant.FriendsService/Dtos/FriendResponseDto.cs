namespace HomeAccountant.FriendsService.Dtos
{
    public class FriendResponseDto
    {
        public int Id { get; set; }
        public required string CreatorId { get; set; }
        public required string RecipientId { get; set; }
        public bool IsRejected { get; set; } = false;
    }
}
