using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using System.Drawing;

namespace HomeAccountant.Core.ViewModels
{
    public class BillingPeriodChartViewModel : BaseViewModel
    {
        private readonly IBillingPeriodService _billingPeriodService;
        private int _billingPeriodId;
        private int _registerid;
        private string? _billingPeriodName;

        public BillingPeriodChartViewModel(IBillingPeriodService billingPeriodService)
        {
            _billingPeriodService = billingPeriodService;

            _chartDatasets = new List<ChartDataset>();
        }

        private BillingPeriodStatisticModel? _billingPeriodStatistic;
        public BillingPeriodStatisticModel? BillingPeriodStatistic
        {
            get => _billingPeriodStatistic;
            set => SetValue(ref _billingPeriodStatistic, value);
        }

        private List<ChartDataset> _chartDatasets;
        public List<ChartDataset> ChartDatasets
        {
            get => _chartDatasets;
            set => SetValue(ref _chartDatasets, value);
        }
        
        public async Task SetParametersAsync(int billingPeriodId, int registerid, string billingPeriodName, CancellationToken cancellationToken)
        {
            _billingPeriodId = billingPeriodId;
            _registerid = registerid;
            _billingPeriodName = billingPeriodName;

            await GetBillingPeriodStatistic(cancellationToken);
            ChartDatasets = GetChartDataset() ?? new List<ChartDataset>();
        }

        public async Task RefreshChart(CancellationToken cancellationToken = default)
        {
            await GetBillingPeriodStatistic(cancellationToken);
            ChartDatasets = GetChartDataset() ?? new List<ChartDataset>();
            NotifyPropertyChangedAsync(nameof(BillingPeriodStatistic));
        }

        public List<ChartDataset>? GetChartDataset()
        {
            if (BillingPeriodStatistic is null || BillingPeriodStatistic.ChartData is null)
            {
                return null;
            }

            return new List<ChartDataset>()
            {
                new ChartDataset(
                    _billingPeriodName ?? string.Empty,
                    BillingPeriodStatistic.ChartData
                        .Select(x => new ChartValue(
                                x.CategoryName ?? throw new ArgumentNullException(nameof(x.CategoryName)),
                                x.Sum,
                                Color.FromArgb(x.ColorA, x.ColorR, x.ColorG, x.ColorB))))
            };
        }

        private async Task GetBillingPeriodStatistic(CancellationToken cancellationToken = default)
        {
            var result = await _billingPeriodService.GetBillingPeriodStatisticAsync(
                _registerid,
                _billingPeriodId,
                cancellationToken);

            if (!result.Result)
            {
                return;
            }

            _billingPeriodStatistic = result.Value;
        }
    }
}
