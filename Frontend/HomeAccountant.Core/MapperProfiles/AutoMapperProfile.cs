using AutoMapper;
using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.MapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<LoginResponseDTO, TokenAuthenticationModel>()
                .ConstructUsing(x => new TokenAuthenticationModel(x.Token, x.RefreshToken));
        }
    }
}
