﻿using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HomeAccountant.Pages.Registers
{
    public partial class RegisterCreate : ComponentBase, IModalDialog<RegisterModel, RegisterModel>
    {
        private RegisterModel? _registerModel;
        private TaskCompletionSource<RegisterModel?>? _tcs;

        private EditContext? _editContext;

        private IModal? _modal;

        private void ClearModal()
        {
            if (_registerModel is null)
                return;

            _registerModel.Clear();
            _editContext = new EditContext(_registerModel);
        }

        private async Task Cancel()
        {
            ClearModal();
            _tcs?.SetResult(null);
            await HideModalAsync();
        }

        private async Task Submit()
        {
            if (_editContext is null)
                return;


            if (!_editContext.Validate())
                return;

            _tcs?.SetResult(_registerModel);

            await HideModalAsync();
        }

        public async Task InitializeDialogAsync(RegisterModel model)
        {
            _registerModel = model;
            _editContext = new EditContext(_registerModel);
            _tcs = new TaskCompletionSource<RegisterModel?>();

            await InvokeAsync(StateHasChanged);
        }

        public async Task<RegisterModel?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null || _tcs is null)
                return null;

            await _modal.ShowModalAsync();
            return await _tcs.Task;
        }

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return;

            await _modal.HideModalAsync();
        }
    }
}
