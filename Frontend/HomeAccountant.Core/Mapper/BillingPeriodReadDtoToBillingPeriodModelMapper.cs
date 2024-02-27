using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class BillingPeriodReadDtoToBillingPeriodModelMapper : ITypeMapper<BillingPeriodModel, BillingPeriodReadDto>
    {
        private readonly ITypeMapper<RegisterModel, RegisterReadDto> _registerMapper;
        private readonly ITypeMapper<EntryModel, EntryReadDto> _entryMapper;

        public BillingPeriodReadDtoToBillingPeriodModelMapper(ITypeMapper<RegisterModel, RegisterReadDto> registerMapper,
            ITypeMapper<EntryModel, EntryReadDto> entryMapper)
        {
            _registerMapper = registerMapper;
            _entryMapper = entryMapper;
        }

        public BillingPeriodModel Map(BillingPeriodReadDto? value)
        {
            value.Protect();

            return new BillingPeriodModel()
            { 
                Id = value!.Id,
                Name = value.Name,
                Register = _registerMapper.Map(value.Register),
                Entries = value.Entries?.Select(_entryMapper.Map),
                IsOpen = value.IsOpen,
                CreationDate = value.CreationDate
            };
        }
    }
}
