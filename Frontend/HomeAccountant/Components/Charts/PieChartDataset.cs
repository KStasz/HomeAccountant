using HomeAccountant.Core.Model;
using System.Drawing;

namespace HomeAccountant.Components.Charts
{
    public class PieChartDataset
    {
        public required string label { get; set; }
        public int hoverOffset { get; set; }
        public required IEnumerable<double> data { get; set; }
        public required IEnumerable<string> backgroundColor { get; set; }
    }
}
