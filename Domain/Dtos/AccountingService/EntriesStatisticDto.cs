namespace Domain.Dtos.AccountingService
{
    public class EntriesStatisticDto
    {
        public double TotalSum { get; set; }
        public IEnumerable<EntriesStatisticChartData>? ChartData { get; set; }
    }
}
