using HomeAccountant.Core.Model;
using HomeAccountant.Core.Extensions;

namespace HomeAccountant.Components.Charts
{
    public class PieChartConfiguration
    {
        public PieChartConfiguration(IEnumerable<ChartDataset> datasets)
        {
            labels = new List<string>(datasets.SelectMany(x => x.ChartValues.Select(x => x.Label)));
            this.datasets = new List<PieChartDataset>(datasets.Select(x => new PieChartDataset()
            {
                label = x.ChartLabel,
                backgroundColor = x.ChartValues.Select(y => y.Color.ConvertToRgb()),
                data = x.ChartValues.Select(y => y.Value),
                hoverOffset = 4
            }));
        }

        public IEnumerable<string> labels { get; set; }
        public IEnumerable<PieChartDataset> datasets { get; set; }
    }
}
