using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class EntriesStatisticChartDataDtoToEntriesStatisticChartDataModelMapper : ITypeMapper<EntriesStatisticChartDataModel, EntriesStatisticChartDataDto>
    {
        public EntriesStatisticChartDataModel Map(EntriesStatisticChartDataDto? value)
        {
            value.Protect();

            return new EntriesStatisticChartDataModel()
            {
                CategoryName = value!.CategoryName,
                ColorA = value!.ColorA,
                ColorB = value!.ColorB,
                ColorG = value!.ColorG,
                ColorR = value!.ColorR,
                Sum = value!.Sum
            };
        }
    }
}
