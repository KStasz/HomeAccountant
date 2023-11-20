using AutoMapper;
using Domain.Dtos.IdentityPlatform;
using Microsoft.AspNetCore.Identity;

namespace HomeAccountant.IdentityPlatform.MapperProfiles
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<IdentityUser, UserModelDto>();
        }
    }
}
