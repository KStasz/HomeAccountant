using AutoMapper;
using HomeAccountant.FriendsService.Dtos;
using HomeAccountant.FriendsService.Model;

namespace HomeAccountant.FriendsService.Profiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateFriendRequestDto, FriendRequest>();
            CreateMap<FriendRequest, FriendResponseDto>();
        }
    }
}
