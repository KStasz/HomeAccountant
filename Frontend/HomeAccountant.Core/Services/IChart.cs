using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IChart
    {
        Task CreateChartAsync(IEnumerable<ChartDataset>? dataset);
        Task DestroyChart();
    }
}