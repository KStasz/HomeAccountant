namespace HomeAccountant.Core.Model
{
    public class ChartDataset
    {
        public ChartDataset(string chartLabel, IEnumerable<ChartValue> chartValues)
        {
            ChartLabel = chartLabel;
            ChartValues = chartValues;
        }

        public string ChartLabel { get; set; }
        public IEnumerable<ChartValue> ChartValues { get; set; }
    }
}
