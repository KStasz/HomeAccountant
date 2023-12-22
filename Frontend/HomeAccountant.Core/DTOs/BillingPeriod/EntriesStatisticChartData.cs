using System.Drawing;

namespace HomeAccountant.Core.DTOs.BillingPeriod
{
    public class EntriesStatisticChartData
    {
        public required string CategoryName { get; set; }
        public double Sum { get; set; }
        public int ColorA { get; set; }
        public int ColorR { get; set; }
        public int ColorG { get; set; }
        public int ColorB { get; set; }
    }
}
