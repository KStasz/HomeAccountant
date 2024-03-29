﻿using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages.Categories
{
    public partial class CategoryDeleteModal : ComponentBase, IModalDialog<CategoryModel>
    {
        private CategoryModel? _model;
        private TaskCompletionSource<ModalResult>? _tcs;
        private IModal? _modal;

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return;

            await _modal.HideModalAsync();
        }

        private async Task Cancel()
        {
            _tcs?.SetResult(ModalResult.Cancel);

            await HideModalAsync();
        }

        private async Task Delete()
        {
            _tcs?.SetResult(ModalResult.Success);

            await HideModalAsync();
        }

        public async Task InitializeDialogAsync(CategoryModel model)
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
