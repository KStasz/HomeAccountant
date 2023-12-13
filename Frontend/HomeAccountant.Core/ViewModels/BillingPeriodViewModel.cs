using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class BillingPeriodViewModel : BaseViewModel
    {
        private readonly IBillingPeriodService _billingPeriodService;
        private int _registerId;
        private int _currentPeriodIndex;

        public BillingPeriodViewModel(IBillingPeriodService billingPeriodService)
        {
            _billingPeriodService = billingPeriodService;
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

        public List<BillingPeriodReadDto>? AvailableBillingPeriods { get; set; }

        public int AvailableBillingPeriodsCount
        {
            get
            {
                return AvailableBillingPeriods?.Count ?? 0;
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

        public async Task ToggleBillingPeriod()
        {
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
            await GetBillingPeriod(_registerId);
        }

        public async Task InitializeAsync(int registerId)
        {
            IsBusy = true;
            _registerId = registerId;

            await GetBillingPeriod(_registerId);
            IsBusy = false;
        }

        public void PreviousPeriod()
        {
            if ((!AvailableBillingPeriods?.Any() ?? false)
                || _currentPeriodIndex == 0)
                return;

            SelectedBillingPeriod = AvailableBillingPeriods?[--_currentPeriodIndex];
        }

        public void NextPeriod()
        {
            if ((!AvailableBillingPeriods?.Any() ?? false)
                || _currentPeriodIndex == AvailableBillingPeriodsCount - 1)
            {
                return;
            }

            SelectedBillingPeriod = AvailableBillingPeriods?[++_currentPeriodIndex];
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
            SelectedBillingPeriod = AvailableBillingPeriods?[_currentPeriodIndex];
        }

    }
}
