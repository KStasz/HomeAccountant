using HomeAccountant.Core.Delegates;
using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using System.Drawing;

namespace HomeAccountant.Core.ViewModels
{
    public class BillingPeriodViewModel : MvvmViewModel
    {
        private readonly IBillingPeriodService _billingPeriodService;
        private int _registerId;
        private int _currentPeriodIndex;

        public BillingPeriodViewModel(IBillingPeriodService billingPeriodService)
        {
            _billingPeriodService = billingPeriodService;
            RefreshChart = ReadStatistic;
        }

        private BillingPeriodReadDto? _selectedBillingPeriod;
        public BillingPeriodReadDto? SelectedBillingPeriod
        {
            get
            {
                return _selectedBillingPeriod;
            }
            set
            {
                _selectedBillingPeriod = value;
                NotifyPropertyChanged();
            }
        }

        public Func<Task> RefreshChart { get; set; }

        public List<BillingPeriodReadDto>? AvailableBillingPeriods { get; set; }

        public int AvailableBillingPeriodsCount
        {
            get
            {
                return AvailableBillingPeriods?.Count ?? 0;
            }
        }

        private bool _isChartVisible = true;
        public bool IsChartVisible
        {
            get
            {
                return _isChartVisible;
            }
            set
            {
                _isChartVisible = value;
                NotifyPropertyChanged();
            }
        }

        public IModalDialog<BillingPeriodCreateDto, BillingPeriodCreateDto>? BillingCreateDialog { get; set; }
        public IAlert? Alert { get; set; }

        public Dictionary<string, object> IsOpenClosePeriodEnabled
        {
            get
            {
                return SelectedBillingPeriod is null ? new Dictionary<string, object>() { { "disabled", "" } } : new Dictionary<string, object>();
            }
        }

        public Dictionary<string, object> IsPreviousPeriodButtonEnabled
        {
            get
            {
                return (!AvailableBillingPeriods?.Any() ?? false) || _currentPeriodIndex == 0 ? new Dictionary<string, object>() { { "disabled", "" } } : new Dictionary<string, object>();
            }
        }

        public Dictionary<string, object> IsNextPeriodButtonEnabled
        {
            get
            {
                return (!AvailableBillingPeriods?.Any() ?? false) || _currentPeriodIndex == AvailableBillingPeriodsCount - 1 ? new Dictionary<string, object>() { { "disabled", "" } } : new Dictionary<string, object>();
            }
        }


        private BillingPeriodStatisticDto? _billingPeriodStatistic;
        public BillingPeriodStatisticDto? BillingPeriodStatistic
        {
            get
            {
                return _billingPeriodStatistic;
            }
            set
            {
                _billingPeriodStatistic = value;
                NotifyPropertyChanged();
            }
        }

        public async Task ReadStatistic()
        {
            IsChartVisible = false;
            if (_selectedBillingPeriod is null)
            {
                return;
            }

            var result = await _billingPeriodService.GetBillingPeriodStatisticAsync(_registerId, _selectedBillingPeriod.Id);

            if (!result.Result)
            {
                _billingPeriodStatistic = null;

                return;
            }

            _billingPeriodStatistic = result.Value;
            IsChartVisible = true;
        }

        public async Task ToggleBillingPeriod()
        {
            IsChartVisible = false;
            if (SelectedBillingPeriod is null)
                return;

            if (SelectedBillingPeriod.IsOpen)
            {
                await _billingPeriodService.CloseBillingPeriodAsync(_registerId, SelectedBillingPeriod.Id);
            }
            else
            {
                await _billingPeriodService.OpenBillingPeriodAsync(_registerId, SelectedBillingPeriod.Id);
            }

            await RefreshBillingPeriod();
            IsChartVisible = true;
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
                    SelectedBillingPeriod!.Name!,
                    BillingPeriodStatistic.ChartData
                        .Select(x => new ChartValue(
                                x.CategoryName,
                                x.Sum,
                                Color.FromArgb(x.ColorA, x.ColorR, x.ColorG, x.ColorB))))
            };
        }

        public async Task CreateBillingDialog()
        {
            if (!AvailableBillingPeriods?.All(x => x.IsOpen == false) ?? false)
            {
                if (Alert is not null)
                    await Alert.ShowAlert("Przed utworzeniem zamknij inne okresy rozliczeniowe", AlertType.Danger);
                return;
            }

            if (BillingCreateDialog is null)
                return;

            await BillingCreateDialog.InitializeDialogAsync(new BillingPeriodCreateDto());
            var result = await BillingCreateDialog.ShowModalAsync();

            if (result is null)
                return;

            await _billingPeriodService.CreateBillingPeriodAsync(_registerId, result);
            await ReadInitialData(_registerId);
        }

        private async Task ReadInitialData(int registerId)
        {
            await GetBillingPeriod(registerId);
            await ReadStatistic();
        }

        public override async Task PageParameterSetAsync(Dictionary<string, object?> parameters)
        {
            IsBusy = true;
            _registerId = GetParameter<int>(parameters["RegisterId"]);

            await ReadInitialData(_registerId);

            IsBusy = false;
        }

        public async Task PreviousPeriod()
        {
            if ((!AvailableBillingPeriods?.Any() ?? false)
                || _currentPeriodIndex == 0)
                return;

            _selectedBillingPeriod = AvailableBillingPeriods?[--_currentPeriodIndex];

            await ReadStatistic();
        }

        public async Task NextPeriod()
        {
            if ((!AvailableBillingPeriods?.Any() ?? false)
                || _currentPeriodIndex == AvailableBillingPeriodsCount - 1)
                return;

            SelectedBillingPeriod = AvailableBillingPeriods?[++_currentPeriodIndex];

            await ReadStatistic();
        }

        public async Task RefreshBillingPeriod()
        {
            var result = await _billingPeriodService.GetBiilingPeriodsAsync(_registerId);

            if (!result.Result)
                return;

            AvailableBillingPeriods = result.Value?.OrderBy(x => x.Id).ToList();
            SelectedBillingPeriod = AvailableBillingPeriods?[_currentPeriodIndex];
        }

        private async Task GetBillingPeriod(int registerId)
        {
            var result = await _billingPeriodService.GetBiilingPeriodsAsync(registerId);

            if (!result.Result)
                return;

            AvailableBillingPeriods = result.Value?.OrderBy(x => x.Id).ToList();

            if (!AvailableBillingPeriods?.Any() ?? false)
            {
                return;
            }

            _currentPeriodIndex = AvailableBillingPeriodsCount - 1;
            _selectedBillingPeriod = AvailableBillingPeriods?[_currentPeriodIndex];
        }
    }
}
