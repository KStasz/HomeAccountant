using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Core.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IRegisterService _registerService;

        public RegisterViewModel(IRegisterService registerService)
        {
            _registerService = registerService;
            AsyncInitialize = InitializeAsync();
        }

        public Task AsyncInitialize { get; set; }
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

        private async Task InitializeAsync()
        {
            IsBusy = true;
            await ReadRegisters();
            IsBusy = false;
        }

        private async Task ReadRegisters()
        {
            var result = await _registerService.GetRegistersAsync();

            if (!result.Result)
            {
                return;
            }

            AvailableRegisters = result.Value;
        }

        public async Task CreateRegister()
        {
            if (CreateRegisterDialog is null)
                return;

            await CreateRegisterDialog.InitializeDialogAsync(new RegisterCreateDto());

            var result = await CreateRegisterDialog.ShowModalAsync();

            if (result is null)
                return;

            var response = await _registerService.CreateRegisterAsync(result);

            if (!response.Result)
            {
                SetErrorMessage("Nie udało się utworzyć księgi.");
            }

            await ReadRegisters();
        }

        public async Task DeleteRegister(RegisterReadDto registerReadDto)
        {
            if (DeleteRegisterDialog is null)
                return;

            await DeleteRegisterDialog.InitializeDialogAsync(registerReadDto);

            var result = await DeleteRegisterDialog.ShowModalAsync();

            if (result == ModalResult.Cancel)
            {
                return;
            }

            await _registerService.DeleteRegisterAsync(registerReadDto);

            await ReadRegisters();
        }
    }
}
