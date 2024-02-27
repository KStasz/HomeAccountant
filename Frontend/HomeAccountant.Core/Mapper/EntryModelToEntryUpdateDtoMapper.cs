using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class EntryModelToEntryUpdateDtoMapper : ITypeMapper<EntryUpdateDto, EntryModel>
    {
        public EntryUpdateDto Map(EntryModel? value)
        {
            value.Protect();

            return new EntryUpdateDto()
            {
                Name = value!.Name,
                Id = value.Id,
                Price = value.Price,
                CategoryId = value.CategoryId
            };
        }
    }
}
