using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.Registers
{
    public partial class RegisterDelete : ComponentBase, IModalDialog<RegisterModel>
    {
        private IModal? _modalReference;
        private RegisterModel? _registerModel;
        private TaskCompletionSource<ModalResult>? _tcs;

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modalReference is null)
                return;

            await _modalReference.HideModalAsync();
        }

        private async Task Cancel()
        {
            _tcs?.SetResult(ModalResult.Cancel);
            await HideModalAsync();
        }

        public async Task InitializeDialogAsync(RegisterModel model)
        {
            _registerModel = model;
            _tcs = new TaskCompletionSource<ModalResult>();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DeleteBook()
        {
            _tcs?.SetResult(ModalResult.Success);

            await HideModalAsync();
        }

        public async Task<ModalResult> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modalReference is null || _tcs is null)
                return ModalResult.Cancel;

            await _modalReference.ShowModalAsync();

            return await _tcs.Task;
        }

    }
}
