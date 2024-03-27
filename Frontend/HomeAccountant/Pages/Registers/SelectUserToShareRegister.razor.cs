using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.Registers
{
    public partial class SelectUserToShareRegister : ComponentBase, IModalDialog<IEnumerable<UserModel>?, UserModel>
    {
        private IModal? _modal;
        private IEnumerable<UserModel>? _model;
        private TaskCompletionSource<UserModel?>? _tcs;

        private async Task Cancel()
        {
            _tcs?.SetResult(null);

            await HideModalAsync();
        }

        private async Task SelectUser(UserModel selectedUser)
        {
            _tcs?.SetResult(selectedUser);

            await HideModalAsync();
        }

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return;

            await _modal.HideModalAsync();
        }

        public async Task InitializeDialogAsync(IEnumerable<UserModel>? model)
        {
            _model = model;
            _tcs = new TaskCompletionSource<UserModel?>();

            await InvokeAsync(StateHasChanged);
        }

        public async Task<UserModel?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null
                || _tcs is null)
                return null;

            await _modal.ShowModalAsync();
            await InvokeAsync(StateHasChanged);
            
            return await _tcs.Task;
        }
    }
}
