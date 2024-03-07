using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.Registers
{
    public partial class SelectUserToShareRegister : ComponentBase, IModalDialog<Task<ServiceResponse<IEnumerable<UserModel>?>>, UserModel>
    {
        private IModal? _modal;
        private Task<ServiceResponse<IEnumerable<UserModel>?>>? _task;
        private ServiceResponse<IEnumerable<UserModel>?>? _model;
        private TaskCompletionSource<UserModel?>? _tcs;
        private bool _isLoading = true;

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

        public async Task InitializeDialogAsync(Task<ServiceResponse<IEnumerable<UserModel>?>> model)
        {
            _task = model;
            _tcs = new TaskCompletionSource<UserModel?>();

            await InvokeAsync(StateHasChanged);
        }

        public async Task<UserModel?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null 
                || _tcs is null
                || _task is null)
                return null;

            await _modal.ShowModalAsync();

            _model = await _task;
            _isLoading = false;
            await InvokeAsync(StateHasChanged);

            return await _tcs.Task;
        }
    }
}
