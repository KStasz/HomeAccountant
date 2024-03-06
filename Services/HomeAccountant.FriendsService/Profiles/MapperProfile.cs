using AutoMapper;
using Domain.Dtos.FriendsService;
using HomeAccountant.FriendsService.Model;

namespace HomeAccountant.FriendsService.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //CreateMap<FriendshipsNotifications, FriendResponseDto>();
            //CreateMap<FriendshipsNotifications, Friendships>()
            //    .ForMember(x => x.UserId, x => x.MapFrom(y => y.CreatorId))
            //    .ForMember(x => x.Id, x => x.Ignore())
            //    .ForMember(x => x.FriendId, x => x.MapFrom(y => y.RecipientId));
        }
    }
}
