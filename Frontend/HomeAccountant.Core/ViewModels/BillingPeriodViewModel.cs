using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Core.ViewModels
{
    public class BillingPeriodViewModel : MvvmViewModel
    {
        private readonly IBillingPeriodService _billingPeriodService;
        private readonly IRegisterService _registerService;
        private readonly NavigationManager _navManager;
        private readonly EntryViewModel _entryViewModel;
        private readonly BillingPeriodChartViewModel _billingPeriodChartViewModel;
        private readonly IBillingPeriodRealTimeService _billingPeriodRealTimeService;
        private int _registerId;
        private int _currentPeriodIndex;

        public BillingPeriodViewModel(IBillingPeriodService billingPeriodService,
            IRegisterService registerService,
            NavigationManager navManager,
            EntryViewModel entryViewModel,
            BillingPeriodChartViewModel billingPeriodChartViewModel,
            IBillingPeriodRealTimeService billingPeriodRealTimeService)
        {
            _billingPeriodService = billingPeriodService;
            _registerService = registerService;
            _navManager = navManager;
            _entryViewModel = entryViewModel;
            _billingPeriodChartViewModel = billingPeriodChartViewModel;
            _billingPeriodRealTimeService = billingPeriodRealTimeService;

            _billingPeriodRealTimeService.EntryCreated += _billingPeriodRealTimeService_EntryCreated;
            _billingPeriodRealTimeService.BillingPeriodStateChanged += BillingPeriodRealTimeService_BillingPeriodStateChanged;
            _entryViewModel.NotifyEntryHasBeenCreated = EntryHasBeenCreated;
        }

        public EntryViewModel EntriesPageViewModel => _entryViewModel;
        public BillingPeriodChartViewModel BillingPeriodChartViewModel => _billingPeriodChartViewModel;

        public List<BillingPeriodModel>? AvailableBillingPeriods { get; set; }

        private BillingPeriodModel? _selectedBillingPeriod;
        public BillingPeriodModel? SelectedBillingPeriod
        {
            get => _selectedBillingPeriod;
            set => SetValue(ref _selectedBillingPeriod, value);
        }

        private RegisterModel? _register;
        public RegisterModel? Register
        {
            get => _register;
            set => SetValue(ref _register, value);
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
            await _billingPeriodRealTimeService.BillingPeriodStateChangedAsync(SelectedBillingPeriod.Id);
        }

        public async Task EntryHasBeenCreated()
        {
            if (SelectedBillingPeriod is null)
                return;

            await _billingPeriodRealTimeService.EntryCreatedAsync(SelectedBillingPeriod.Id, CancellationToken);
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

            await ReadRegister();
            await ReadInitialDataAsync(_registerId, CancellationToken);
            await _entryViewModel.SetParametersAsync(SelectedBillingPeriod?.Id ?? 0, _registerId, SelectedBillingPeriod?.IsOpen ?? false);
            await _billingPeriodChartViewModel.SetParametersAsync(
                SelectedBillingPeriod?.Id ?? 0,
                _registerId,
                SelectedBillingPeriod?.Name ?? string.Empty,
                CancellationToken);
            await _billingPeriodRealTimeService.InitializeAsync(CancellationToken);

            IsBusy = false;
        }

        public async Task PreviousPeriodAsync()
        {
            if ((!AvailableBillingPeriods?.Any() ?? false)
                || _currentPeriodIndex == 0)
                return;

            SelectedBillingPeriod = AvailableBillingPeriods?[--_currentPeriodIndex];

            await _entryViewModel.SetParametersAsync(
                SelectedBillingPeriod?.Id ?? 0,
                _registerId,
                SelectedBillingPeriod?.IsOpen ?? false);
            await _billingPeriodChartViewModel.SetParametersAsync(
                SelectedBillingPeriod?.Id ?? 0,
                _registerId,
                SelectedBillingPeriod?.Name ?? string.Empty,
                CancellationToken);
        }

        public async Task NextPeriodAsync()
        {
            if ((!AvailableBillingPeriods?.Any() ?? false)
                || _currentPeriodIndex == AvailableBillingPeriodsCount - 1)
                return;

            SelectedBillingPeriod = AvailableBillingPeriods?[++_currentPeriodIndex];

            await _entryViewModel.SetParametersAsync(
                SelectedBillingPeriod?.Id ?? 0,
                _registerId,
                SelectedBillingPeriod?.IsOpen ?? false);
            await _billingPeriodChartViewModel.SetParametersAsync(
                SelectedBillingPeriod?.Id ?? 0,
                _registerId,
                SelectedBillingPeriod?.Name ?? string.Empty,
                CancellationToken);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            _billingPeriodRealTimeService.EntryCreated -= _billingPeriodRealTimeService_EntryCreated;
            await _billingPeriodRealTimeService.DisposeAsync().ConfigureAwait(false);
            
            await base.DisposeAsyncCore();
        }

        private async Task ReadRegister(CancellationToken cancellationToken = default)
        {
            var response = await _registerService.GetRegister(_registerId, cancellationToken);

            if (!response.Result)
            {
                if (Alert is not null)
                    await Alert.ShowAlertAsync(
                        response.Errors.JoinToString(),
                        AlertType.Danger,
                        cancellationToken);

                Register = null;

                _navManager.NavigateTo("books", false);
            }

            Register = response.Value;
        }

        private async Task RefreshBillingPeriodAsync(CancellationToken cancellationToken = default)
        {
            var result = await _billingPeriodService.GetBiilingPeriodsAsync(_registerId, cancellationToken);

            if (!result.Result)
                return;

            AvailableBillingPeriods = result.Value?.OrderBy(x => x.Id).ToList();
            SelectedBillingPeriod = AvailableBillingPeriods?[_currentPeriodIndex];
        }

        private async Task ReadInitialDataAsync(int registerId, CancellationToken cancellationToken = default)
        {
            await GetBillingPeriodAsync(registerId, cancellationToken);
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

                AvailableBillingPeriods = null;
                SelectedBillingPeriod = null;

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

        private async Task _billingPeriodRealTimeService_EntryCreated(object sender, RealTimeEventArgs<int> e)
        {
            if (SelectedBillingPeriod is null)
                return;

            if (SelectedBillingPeriod.Id != e.Value)
                return;

            await RefreshBillingPeriodAsync();
            await _entryViewModel.RefreshDataAsync();
            await _billingPeriodChartViewModel.RefreshChart();
        }

        private async Task BillingPeriodRealTimeService_BillingPeriodStateChanged(object sender, RealTimeEventArgs<int> e)
        {
            await RefreshBillingPeriodAsync();
            _entryViewModel.RefreshBillingPeriodState(SelectedBillingPeriod?.IsOpen ?? false);
        }
    }
}
