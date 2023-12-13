﻿using AutoMapper;
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
                .ForMember(x => x.Period, x => x.MapFrom(y => y.BillingPeriod))
                .ForMember(x => x.Creator, x => x.MapFrom(y => y.CreatedBy));

            CreateMap<EntryUpdateDto, Entry>();

            CreateMap<BillingPeriod, BillingPeriodReadDto>();
            CreateMap<BillingPeriodCreateDto, BillingPeriod>();
            CreateMap<BillingPeriodUpdateDto, BillingPeriod>();
        }
    }
}
