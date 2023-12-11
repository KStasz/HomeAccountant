﻿using HomeAccountant.Core.DTOs;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages
{
    public partial class BookDelete : ComponentBase, IModalDialog<RegisterReadDto>
    {
        private IModal? _modalReference;
        private RegisterReadDto? _registerModel;
        private TaskCompletionSource<ModalResult>? _tcs;

        public async Task HideModalAsync()
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

        public async Task InitializeDialogAsync(RegisterReadDto model)
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

        public async Task<ModalResult> ShowModalAsync()
        {
            await _modalReference?.ShowModalAsync();

            return await _tcs?.Task;
        }

    }
}
