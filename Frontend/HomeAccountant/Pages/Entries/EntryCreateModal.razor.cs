﻿using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.DTOs.Entry;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HomeAccountant.Pages.Entries
{
    public partial class EntryCreateModal : ComponentBase, IModalDialog<EntryCreateDto, EntryCreateDto>
    {
        private IModal? _modal;
        private EntryCreateDto? _model;
        private TaskCompletionSource<EntryCreateDto?>? _tcs;
        private EditContext? _editContext;

        [Parameter]
        public IEnumerable<CategoryReadDto>? Categories { get; set; }

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return;

            await _modal.HideModalAsync();
        }

        private async Task Cancel()
        {
            ClearModel();
            _tcs?.SetResult(null);
            await HideModalAsync();
        }

        private async void SaveChanges()
        {
            if (!_editContext?.Validate() ?? false)
                return;

            _tcs?.SetResult(_model);

            await HideModalAsync();
        }

        private void ClearModel()
        {
            if (_model is null)
                return;

            _model.Clear();
            _editContext = new EditContext(_model);
        }

        public async Task InitializeDialogAsync(EntryCreateDto model)
        {
            _model = model;
            _tcs = new TaskCompletionSource<EntryCreateDto?>();
            _editContext = new EditContext(_model);

            await InvokeAsync(StateHasChanged);
        }

        public async Task<EntryCreateDto?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null || _tcs is null)
                return null;

            await _modal.ShowModalAsync();

            return await _tcs.Task;
        }
    }
}
