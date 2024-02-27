using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Mapper;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HomeAccountant.Pages.Authentication
{
    public partial class LoginPage : ComponentBase, IModalDialog<LoginModel, LoginResponseModel>
    {
        [Parameter]
        public IAuthenticationService? AuthService { get; set; }

        private LoginModel? _loginData;
        private TaskCompletionSource<LoginResponseModel?>? _tcs;
        bool _isBusy = false;
        private IModal? _modal;
        private EditContext? _editContext;
        private IAlert? _alert;

        private void ClearModal()
        {
            if (_loginData is null)
                return;

            _loginData.Clear();
            _editContext = new EditContext(_loginData);
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

            var result = await AuthService.LoginAsync(
                _loginData!.Email!,
                _loginData!.Password!);

            if (!result.Result)
            {
                if (_alert is null)
                {
                    _isBusy = false;

                    return;
                }

                await _alert.ShowAlertAsync(
                    $"Wystąpił błąd podczas logowania: " +
                    $"{Environment.NewLine}{string.Join(Environment.NewLine, result!.Errors ?? new string[0])}",
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

        public async Task InitializeDialogAsync(LoginModel model)
        {
            _loginData = model;
            _editContext = new EditContext(_loginData);
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
