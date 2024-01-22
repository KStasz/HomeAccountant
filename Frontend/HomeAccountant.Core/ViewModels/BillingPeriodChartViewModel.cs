using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using System.Drawing;

namespace HomeAccountant.Core.ViewModels
{
    public class BillingPeriodChartViewModel : MvvmViewModel
    {
        private readonly IBillingPeriodService _billingPeriodService;

        private int _billingPeriodId;
        private int _registerid;
        private string? _billingPeriodName;

        public BillingPeriodChartViewModel(IBillingPeriodService billingPeriodService)
        {
            _billingPeriodService = billingPeriodService;
        }

        private BillingPeriodStatisticDto? _billingPeriodStatistic;
        public BillingPeriodStatisticDto? BillingPeriodStatistic
        {
            get => _billingPeriodStatistic;
            set => SetValue(ref _billingPeriodStatistic, value);
        }

        public override async Task PageParameterSetAsync(Dictionary<string, object?> parameters)
        {
            _billingPeriodId = GetParameter<int>(parameters["BillingPeriodId"]);
            _registerid = GetParameter<int>(parameters["Registerid"]);
            _billingPeriodName = GetParameter<string>(parameters["BillingPeriodName"]);

            await GetBillingPeriodStatistic(CancellationToken);
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
                                x.CategoryName,
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
