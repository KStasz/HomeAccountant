using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HomeAccountant.Pages.Authentication
{
    public partial class RegisterPage : ComponentBase, IModalDialog<RegisterUserDto, LoginResponseDTO>
    {
        [Parameter]
        public IAuthenticationService? AuthService { get; set; }

        private IAlert? _alert;
        private RegisterUserDto? _registerUserDto;
        private TaskCompletionSource<LoginResponseDTO?>? _tcs;
        private bool _isBusy;
        private EditContext? _editContext;
        private IModal? _modal;


        private void ClearModal()
        {
            if (_registerUserDto is null)
                return;

            _registerUserDto.Clear();
            _editContext = new EditContext(_registerUserDto);
        }

        private async Task Cancel()
        {
            ClearModal();
            _tcs?.SetResult(null);
            await HideModalAsync();
        }

        private async Task Submit()
        {
            _isBusy = true;
            if (_editContext is null)
            {
                _isBusy = false;

                return;
            }


            if (!_editContext.Validate())
            {
                _isBusy = false;

                return;
            }

            if (AuthService is null)
            {
                _isBusy = false;

                return;
            }

            var result = await AuthService.RegisterAsync(
                _registerUserDto!.Email!,
                _registerUserDto!.UserName!,
                _registerUserDto!.Password!);

            if (!result.Result)
            {
                await _alert!.ShowAlertAsync($"Nie udało się utworzyć konta: {Environment.NewLine}{string.Join(Environment.NewLine, result.Errors)}", AlertType.Danger);

                _isBusy = false;
                return;
            }

            _tcs?.SetResult(result.Value);
            _isBusy = false;
        }

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return;

            await _modal.HideModalAsync();
        }

        public async Task InitializeDialogAsync(RegisterUserDto model)
        {
            _registerUserDto = model;
            _editContext = new EditContext(_registerUserDto);
            _tcs = new TaskCompletionSource<LoginResponseDTO?>();

            await InvokeAsync(StateHasChanged);
        }

        public async Task<LoginResponseDTO?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null || _tcs is null)
                return null;

            await _modal.ShowModalAsync();

            return await _tcs.Task;
        }
    }
}
