using AutoMapper;
using Domain.Dtos.AccountingService;
using HomeAccountant.AccountingService.Models;

namespace HomeAccountant.AccountingService.Profiles
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Register, RegisterReadDto>();
            CreateMap<RegisterCreateDto, Register>();
            CreateMap<RegisterUpdateDto, Register>();

            CreateMap<EntryCreateDto, Entry>();
            CreateMap<Entry, EntryReadDto>()
                .ForMember(x => x.Register, x => x.MapFrom(y => y.Register))
                .ForMember(x => x.Creator, x => x.MapFrom(y => y.CreatedBy));

            CreateMap<EntryUpdateDto, Entry>();
        }
    }
}
