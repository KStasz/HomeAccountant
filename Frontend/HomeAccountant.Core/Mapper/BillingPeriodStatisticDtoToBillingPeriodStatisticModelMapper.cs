using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Mapper
{
    public class BillingPeriodStatisticDtoToBillingPeriodStatisticModelMapper : ITypeMapper<BillingPeriodStatisticModel, BillingPeriodStatisticDto>
    {
        private readonly ITypeMapper<EntriesStatisticChartDataModel, EntriesStatisticChartDataDto> _mapper;

        public BillingPeriodStatisticDtoToBillingPeriodStatisticModelMapper(
            ITypeMapper<EntriesStatisticChartDataModel, EntriesStatisticChartDataDto> mapper)
        {
            _mapper = mapper;
        }

        public BillingPeriodStatisticModel Map(BillingPeriodStatisticDto? value)
        {
            value.Protect();

            return new BillingPeriodStatisticModel()
            {
                ChartData = value!.ChartData?.Select(_mapper.Map),
                TotalSum = value.TotalSum
            };
        }
    }
}
