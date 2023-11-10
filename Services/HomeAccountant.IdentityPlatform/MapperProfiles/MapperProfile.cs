using AutoMapper;
using HomeAccountant.IdentityPlatform.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Logging;

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
