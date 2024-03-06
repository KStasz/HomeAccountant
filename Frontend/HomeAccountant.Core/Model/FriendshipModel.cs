using HomeAccountant.Core.DTOs.Identity;

namespace HomeAccountant.Core.Model
{
    public class FriendshipModel
    {
        public int Id { get; set; }
        public UserModelReadDto? User { get; set; }
        public UserModelReadDto? Friend { get; set; }
        public bool IsAccepted { get; set; }
        public UserModelReadDto? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
