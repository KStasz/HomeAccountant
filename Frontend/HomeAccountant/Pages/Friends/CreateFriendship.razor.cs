using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace HomeAccountant.Pages.Friends
{
    public partial class CreateFriendship : ComponentBase, IModalDialog<string, string>
    {   
        private FriendRequestModel? _model;

        private IModal? _modal;
        private TaskCompletionSource<string?>? _tcs;
        private EditContext? _editContext;

        private async Task Accept()
        {
            if (!_editContext?.Validate() ?? true)
                return;

            _tcs?.SetResult(_model?.Email);
            await HideModalAsync();
        }

        private async Task Cancel()
        {
            ClearModel();
            _tcs?.SetResult(null);
            await HideModalAsync();
        }

        private void ClearModel()
        {
            if (_model is null)
                return;

            _model.Clear();
            _editContext = new EditContext(_model);
        }

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return;

            await _modal.HideModalAsync(cancellationToken);
        }

        public async Task InitializeDialogAsync(string model)
        {
            _model = new FriendRequestModel();
            _tcs = new TaskCompletionSource<string?>();
            _editContext = new EditContext(_model);

            await InvokeAsync(StateHasChanged);
        }

        public async Task<string?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null || _tcs is null)
                return null;

            await _modal.ShowModalAsync(cancellationToken);
            cancellationToken.Register(() => _tcs.TrySetCanceled());

            return await _tcs.Task;
        }

        private class FriendRequestModel : IClearableObject
        {
            [Required(ErrorMessage = "Pole jest wymagane")]
            [EmailAddress(ErrorMessage = "Adres email jest niepoprawny")]
            public string? Email { get; set; }

            public void Clear()
            {
                Email = null;
            }
        }
    }
}
