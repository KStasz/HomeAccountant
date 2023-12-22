using HomeAccountant.Core.DTOs.Register;
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

        public IModalDialog<RegisterReadDto>? DeleteRegisterDialog { get; set; }
        public IModalDialog<RegisterCreateDto, RegisterCreateDto>? CreateRegisterDialog { get; set; }

        private IEnumerable<RegisterReadDto>? _availableRegisters;
        public IEnumerable<RegisterReadDto>? AvailableRegisters
        {
            get
            {
                return _availableRegisters;
            }
            set
            {
                _availableRegisters = value;
                NotifyPropertyChangedAsync();
            }
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

            await CreateRegisterDialog.InitializeDialogAsync(new RegisterCreateDto());

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

        public async Task DeleteRegisterAsync(RegisterReadDto registerReadDto)
        {
            if (DeleteRegisterDialog is null)
                return;

            await DeleteRegisterDialog.InitializeDialogAsync(registerReadDto);

            var result = await DeleteRegisterDialog.ShowModalAsync(CancellationToken);

            if (result == ModalResult.Cancel)
            {
                return;
            }

            await _registerService.DeleteRegisterAsync(registerReadDto, CancellationToken);

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
