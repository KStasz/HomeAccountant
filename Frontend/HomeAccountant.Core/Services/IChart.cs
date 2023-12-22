using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IChart
    {
        Task CreateChartAsync(IEnumerable<ChartDataset>? dataset, CancellationToken cancellationToken = default);
        Task DestroyChartAsync(CancellationToken cancellationToken = default);
    }
}