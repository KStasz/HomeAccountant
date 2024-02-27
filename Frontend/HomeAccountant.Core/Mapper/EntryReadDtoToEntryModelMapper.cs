using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class EntryReadDtoToEntryModelMapper : ITypeMapper<EntryModel, EntryReadDto>
    {
        private readonly ITypeMapper<CategoryModel, CategoryReadDto> _categoryMapper;

        public EntryReadDtoToEntryModelMapper(
            ITypeMapper<CategoryModel, CategoryReadDto> categoryMapper)
        {
            _categoryMapper = categoryMapper;
        }

        public EntryModel Map(EntryReadDto? value)
        {
            value.Protect();

            return new EntryModel()
            {
                Id = value!.Id,
                Name = value.Name,
                Price = value.Price,
                CategoryId = value.Category?.Id ?? 0,
                Category = _categoryMapper.Map(value.Category),
                CreatedDate = value.CreatedDate,
                Creator = value.Creator,
                Period = value.Period is null ? null : new BillingPeriodModel() 
                {
                    Id = value.Period.Id,
                    Name = value.Period.Name,
                    Register = null,
                    Entries = null,
                    IsOpen = value.Period.IsOpen,
                    CreationDate = value.Period.CreationDate
                }
            };
        }
    }
}
