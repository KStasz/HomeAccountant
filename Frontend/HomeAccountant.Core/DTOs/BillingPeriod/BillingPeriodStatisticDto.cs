using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.DTOs.BillingPeriod
{
    public class BillingPeriodStatisticDto
    {
        public double TotalSum { get; set; }
        public IEnumerable<EntriesStatisticChartData>? ChartData { get; set; }
    }
}
