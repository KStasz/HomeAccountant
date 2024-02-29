using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using System.Reflection.Metadata.Ecma335;

namespace HomeAccountant.Core.ViewModels
{
    public class BillingPeriodViewModel : MvvmViewModel
    {
        private readonly IBillingPeriodService _billingPeriodService;
        private readonly IPubSubService _pubSubService;
        private int _registerId;
        private int _currentPeriodIndex;

        public BillingPeriodViewModel(IBillingPeriodService billingPeriodService,
            IPubSubService pubSubService)
        {
            _billingPeriodService = billingPeriodService;
            _pubSubService = pubSubService;
            _pubSubService.MessageSender += _pubSubService_MessageSender;
        }
        public List<BillingPeriodModel>? AvailableBillingPeriods { get; set; }

        private BillingPeriodModel? _selectedBillingPeriod;
        public BillingPeriodModel? SelectedBillingPeriod
        {
            get => _selectedBillingPeriod;
            set => SetValue(ref _selectedBillingPeriod, value);
        }

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
            get => _isChartVisible;
            set => SetValue(ref _isChartVisible, value);
        }

        public IModalDialog<BillingPeriodModel, BillingPeriodModel>? BillingCreateDialog { get; set; }
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

        public async Task ToggleBillingPeriodAsync()
        {
            if (SelectedBillingPeriod is null)
                return;

            ServiceResponse? togglingResponse;

            if (SelectedBillingPeriod.IsOpen)
            {
                togglingResponse = await _billingPeriodService.CloseBillingPeriodAsync(_registerId, SelectedBillingPeriod.Id, CancellationToken);
            }
            else
            {
                togglingResponse = await _billingPeriodService.OpenBillingPeriodAsync(_registerId, SelectedBillingPeriod.Id, CancellationToken);
            }

            if (!togglingResponse.Result)
            {
                if (Alert is not null)
                {
                    await Alert.ShowAlertAsync(
                        string.Join(
                            Environment.NewLine,
                            togglingResponse.Errors ?? Array.Empty<string>()),
                        AlertType.Danger,
                        CancellationToken);
                }
            }

            await RefreshBillingPeriodAsync(CancellationToken);
        }

        public async Task CreateBillingDialogAsync()
        {
            if (!AvailableBillingPeriods?.All(x => x.IsOpen == false) ?? false)
            {
                if (Alert is not null)
                    await Alert.ShowAlertAsync("Przed utworzeniem zamknij inne okresy rozliczeniowe", AlertType.Danger);
                
                return;
            }

            if (BillingCreateDialog is null)
                return;

            await BillingCreateDialog.InitializeDialogAsync(new BillingPeriodModel());
            var result = await BillingCreateDialog.ShowModalAsync(CancellationToken);

            if (result is null)
                return;

            await _billingPeriodService.CreateBillingPeriodAsync(
                _registerId,
                result,
                CancellationToken);

            await ReadInitialDataAsync(
                _registerId,
                CancellationToken);
        }

        public override async Task PageParameterSetAsync(Dictionary<string, object?> parameters)
        {
            IsBusy = true;
            _registerId = GetParameter<int>(parameters["RegisterId"]);

            await ReadInitialDataAsync(_registerId, CancellationToken);

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
                return;

            SelectedBillingPeriod = AvailableBillingPeriods?[++_currentPeriodIndex];
        }

        private async Task RefreshBillingPeriodAsync(CancellationToken cancellationToken = default)
        {
            var result = await _billingPeriodService.GetBiilingPeriodsAsync(_registerId, cancellationToken);

            if (!result.Result)
                return;

            AvailableBillingPeriods = result.Value?.OrderBy(x => x.Id).ToList();

            await Task.Run(() =>
            {
                SelectedBillingPeriod = null;
                SelectedBillingPeriod = AvailableBillingPeriods?[_currentPeriodIndex];
            });
        }

        private async Task ReadInitialDataAsync(int registerId, CancellationToken cancellationToken = default)
        {
            await GetBillingPeriodAsync(registerId, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            _pubSubService.MessageSender -= _pubSubService_MessageSender;
            base.Dispose(disposing);
        }

        private async Task _pubSubService_MessageSender(object? sender, EventArgs e)
        {
            await RefreshBillingPeriodAsync();
        }

        private async Task GetBillingPeriodAsync(int registerId, CancellationToken cancellationToken = default)
        {
            var result = await _billingPeriodService.GetBiilingPeriodsAsync(registerId, cancellationToken);

            if (!result.Result)
            {
                if (Alert is null)
                    return;

                await Alert.ShowAlertAsync(
                    string.Join(
                        Environment.NewLine,
                        result.Errors ?? Array.Empty<string>()),
                    AlertType.Danger,
                    cancellationToken);

                return;
            }

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
