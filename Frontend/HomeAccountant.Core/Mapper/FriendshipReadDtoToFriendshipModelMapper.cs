using HomeAccountant.Core.DTOs.Friends;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class FriendshipReadDtoToFriendshipModelMapper : ITypeMapper<FriendshipModel, FriendshipReadDto>
    {
        public FriendshipModel Map(FriendshipReadDto? value)
        {
            value.Protect();

            return new FriendshipModel()
            {
                Id = value!.Id,
                User = value.User,
                CreatedBy = value.CreatedBy,
                CreationDate = value.CreationDate,
                Friend = value.Friend,
                IsAccepted = value.IsAccepted
            };
        }
    }
}
