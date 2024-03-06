using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;

namespace HomeAccountant.Pages.Friends
{
    public partial class DeleteFriendship : IModalDialog<FriendshipModel>
    {
        private FriendshipModel? _model;
        private TaskCompletionSource<ModalResult>? _tcs;
        private IModal? _modal;

        private async Task Delete()
        {
            _tcs?.SetResult(ModalResult.Success);

            await HideModalAsync();
        }

        private async Task Cancel()
        {
            _tcs?.SetResult(ModalResult.Cancel);

            await HideModalAsync();
        }

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return;

            await _modal.HideModalAsync(cancellationToken);
        }

        public async Task InitializeDialogAsync(FriendshipModel model)
        {
            _model = model;
            _tcs = new TaskCompletionSource<ModalResult>();

            await InvokeAsync(StateHasChanged);
        }

        public async Task<ModalResult> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null || _tcs is null)
                return ModalResult.Cancel;

            await _modal.ShowModalAsync(cancellationToken);
            
            return await _tcs.Task;
        }
    }
}
