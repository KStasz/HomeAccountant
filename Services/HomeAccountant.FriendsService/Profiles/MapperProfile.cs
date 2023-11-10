using AutoMapper;
using HomeAccountant.FriendsService.Dtos;
using HomeAccountant.FriendsService.Model;

namespace HomeAccountant.FriendsService.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<FriendRequest, FriendResponseDto>();
            CreateMap<FriendRequest, Friend>()
                .ForMember(x => x.UserId, x => x.MapFrom(y => y.CreatorId))
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.FriendId, x => x.MapFrom(y => y.RecipientId));
        }
    }
}
