namespace HomeAccountant.Core.Model
{
    public class BillingPeriodStatisticModel
    {
        public double TotalSum { get; set; }
        public IEnumerable<EntriesStatisticChartDataModel>? ChartData { get; set; }
    }
}
