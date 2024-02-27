using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.DTOs.BillingPeriod
{
    public record BillingPeriodStatisticDto
    {
        public double TotalSum { get; set; }
        public IEnumerable<EntriesStatisticChartDataDto>? ChartData { get; set; }
    }
}
