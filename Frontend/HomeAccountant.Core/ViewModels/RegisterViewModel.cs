using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class RegisterViewModel : MvvmViewModel
    {
        private readonly IRegisterService _registerService;

        public RegisterViewModel(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        public IModalDialog<RegisterModel>? DeleteRegisterDialog { get; set; }
        public IModalDialog<RegisterModel, RegisterModel>? CreateRegisterDialog { get; set; }

        private IEnumerable<RegisterModel>? _availableRegisters;
        public IEnumerable<RegisterModel>? AvailableRegisters
        {
            get => _availableRegisters;
            set => SetValue(ref _availableRegisters, value);
        }

        public override async Task PageInitializedAsync()
        {
            IsBusy = true;
            await ReadRegistersAsync(CancellationToken);
            IsBusy = false;
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

            AvailableRegisters = result.Value;
        }
    }
}
