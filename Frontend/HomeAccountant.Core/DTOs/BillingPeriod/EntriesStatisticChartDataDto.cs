namespace HomeAccountant.Core.DTOs.BillingPeriod
{
    public record EntriesStatisticChartDataDto
    {
        public string? CategoryName { get; init; }
        public double Sum { get; init; }
        public int ColorA { get; init; }
        public int ColorR { get; init; }
        public int ColorG { get; init; }
        public int ColorB { get; init; }
    }
}
