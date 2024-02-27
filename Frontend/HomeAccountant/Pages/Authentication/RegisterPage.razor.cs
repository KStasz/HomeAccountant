using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Extensions;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HomeAccountant.Pages.Authentication
{
    public partial class RegisterPage : ComponentBase, IModalDialog<RegisterUserModel, LoginResponseModel>
    {
        [Parameter]
        public IAuthenticationService? AuthService { get; set; }

        private IAlert? _alert;
        private RegisterUserModel? _registerUser;
        private TaskCompletionSource<LoginResponseModel?>? _tcs;
        private bool _isBusy;
        private EditContext? _editContext;
        private IModal? _modal;


        private void ClearModal()
        {
            if (_registerUser is null)
                return;

            _registerUser.Clear();
            _editContext = new EditContext(_registerUser);
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
            if (_editContext is null
                || AuthService is null
                || _registerUser is null)
            {
                _isBusy = false;

                return;
            }

            if (!_editContext.Validate())
            {
                _isBusy = false;

                return;
            }

            var result = await AuthService.RegisterAsync(
                _registerUser.Email!,
                _registerUser.UserName!,
                _registerUser.Password!);

            if (!result.Result)
            {
                await _alert!.ShowAlertAsync(
                    $"Nie udało się utworzyć konta: " +
                    $"{Environment.NewLine}{string.Join(Environment.NewLine, result.Errors ?? Array.Empty<string>())}",
                    AlertType.Danger);

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

        public async Task InitializeDialogAsync(RegisterUserModel model)
        {
            _registerUser = model;
            _editContext = new EditContext(_registerUser);
            _tcs = new TaskCompletionSource<LoginResponseModel?>();

            await InvokeAsync(StateHasChanged);
        }

        public async Task<LoginResponseModel?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null || _tcs is null)
                return null;

            await _modal.ShowModalAsync();

            return await _tcs.Task;
        }
    }
}
