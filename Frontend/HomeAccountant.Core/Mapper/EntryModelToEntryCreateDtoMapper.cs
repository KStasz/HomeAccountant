using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class EntryModelToEntryCreateDtoMapper : ITypeMapper<EntryCreateDto, EntryModel>
    {
        public EntryCreateDto Map(EntryModel? value)
        {
            value.Protect();

            return new EntryCreateDto()
            {
                Name = value!.Name,
                CategoryId = value.CategoryId,
                Price = value.Price
            };
        }
    }
}
