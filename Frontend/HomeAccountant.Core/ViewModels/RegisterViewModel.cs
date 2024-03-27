using System.Collections.ObjectModel;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.Extensions.Logging;

namespace HomeAccountant.Core.ViewModels
{
    public class RegisterViewModel : MvvmViewModel
    {
        private readonly IRegisterService _registerService;
        private readonly IFriendshipService _friendshipService;
        private readonly ILogger<RegisterViewModel> _logger;

        public RegisterViewModel(
            IRegisterService registerService,
            IFriendshipService friendshipService,
            ILogger<RegisterViewModel> logger)
        {
            _registerService = registerService;
            _friendshipService = friendshipService;
            _logger = logger;

            _logger.LogInformation($"{Environment.NewLine}-->Test{Environment.NewLine}");
        }

        public IAlert? Alert { get; set; }
        public IModalDialog<RegisterModel>? DeleteRegisterDialog { get; set; }
        public IModalDialog<RegisterModel, RegisterModel>? CreateRegisterDialog { get; set; }
        public IModalDialog<IEnumerable<UserModel>?, UserModel>? SelectUserDialog { get; set; }

        private ObservableCollection<RegisterModel>? _availableRegisters;
        public ObservableCollection<RegisterModel>? AvailableRegisters
        {
            get => _availableRegisters;
            set => SetValue(ref _availableRegisters, value);
        }

        private ServiceResponse<IEnumerable<RegisterModel>?>? _sharedRegisters;
        public ServiceResponse<IEnumerable<RegisterModel>?>? SharedRegisters
        {
            get => _sharedRegisters;
            set => SetValue(ref _sharedRegisters, value);
        }

        private bool _sharedRegistersLoading = false;
        public bool SharedRegistersLoading
        {
            get => _sharedRegistersLoading;
            set => SetValue(ref _sharedRegistersLoading, value);
        }

        public override async Task PageInitializedAsync()
        {
            IsBusy = true;
            SharedRegistersLoading = true;
            await ReadRegistersAsync(CancellationToken);
            IsBusy = false;
            await ReadSharedRegisters(CancellationToken);
            SharedRegistersLoading = false;
        }

        private async Task ReadSharedRegisters(CancellationToken cancellationToken)
        {
            SharedRegisters = await _registerService.GetSharedRegisters(cancellationToken);
        }

        public async Task CreateRegisterAsync()
        {
            if (CreateRegisterDialog is null)
                return;

            await CreateRegisterDialog.InitializeDialogAsync(new RegisterModel());

            var result = await CreateRegisterDialog.ShowModalAsync(CancellationToken);

            if (result is null)
                return;

            var response = await _registerService.CreateRegisterAsync(result, CancellationToken);

            if (!response.Result)
            {
                SetErrorMessage("Nie udało się utworzyć księgi.");
            }

            await ReadRegistersAsync(CancellationToken);
        }

        public async Task ShareRegister(RegisterModel registerModel)
        {
            if (SelectUserDialog is null)
                return;

            IEnumerable<UserModel>? friends = await LoadFriendsCollection(registerModel);

            await SelectUserDialog.InitializeDialogAsync(friends?.ExceptBy(registerModel.UserIds ?? Array.Empty<string>(), x => x.Id));
            var selectedUser = await SelectUserDialog.ShowModalAsync();

            if (selectedUser is null
                || string.IsNullOrWhiteSpace(selectedUser.Id))
                return;

            var response = await _registerService.ShareRegisterAsync(
                registerModel.Id,
                selectedUser.Id,
                CancellationToken);

            if (!response.Result)
            {
                if (Alert is null)
                    return;

                await Alert.ShowAlertAsync(
                    response.Errors.JoinToString(),
                    AlertType.Danger,
                    CancellationToken);
            }
        }

        private async Task<IEnumerable<UserModel>?> LoadFriendsCollection(RegisterModel registerModel)
        {
            registerModel.AreFriendsLoading = true;
            var friendsResponse = await _friendshipService.GetFriendsAsync();;
            registerModel.AreFriendsLoading = false;
            NotifyPropertyChangedAsync(nameof(registerModel.AreFriendsLoading));
            
            return await friendsResponse.GetValue(Alert);
        }

        public async Task DeleteRegisterAsync(RegisterModel registerModel)
        {
            if (DeleteRegisterDialog is null)
                return;

            await DeleteRegisterDialog.InitializeDialogAsync(registerModel);

            var result = await DeleteRegisterDialog.ShowModalAsync(CancellationToken);

            if (result == ModalResult.Cancel)
            {
                return;
            }

            await _registerService.DeleteRegisterAsync(registerModel.Id, CancellationToken);

            await ReadRegistersAsync(CancellationToken);
        }

        private async Task ReadRegistersAsync(CancellationToken cancellationToken = default)
        {
            var result = await _registerService.GetRegistersAsync(cancellationToken);

            if (!result.Result)
            {
                return;
            }

            AvailableRegisters = new ObservableCollection<RegisterModel>(result.Value ?? Array.Empty<RegisterModel>());
        }
    }
}
