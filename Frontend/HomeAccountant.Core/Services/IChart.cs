using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IChart
    {
        Task CreateChart(IEnumerable<ChartDataset> dataset);
    }
}