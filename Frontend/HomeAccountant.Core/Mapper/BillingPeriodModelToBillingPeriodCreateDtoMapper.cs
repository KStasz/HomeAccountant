using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class BillingPeriodModelToBillingPeriodCreateDtoMapper : ITypeMapper<BillingPeriodCreateDto, BillingPeriodModel>
    {
        public BillingPeriodCreateDto Map(BillingPeriodModel? value)
        {
            value.Protect();

            return new BillingPeriodCreateDto()
            {
                Name = value!.Name
            };
        }
    }
}
