using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HomeAccountant.Pages.Categories
{
    public partial class CategoryCreateModal : ComponentBase, IModalDialog<CategoryCreateDto, CategoryCreateDto>
    {
        private CategoryCreateDto? _model;
        private TaskCompletionSource<CategoryCreateDto?>? _tcs;
        private IModal? _modal;
        private EditContext? _editContext;

        public async Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return;

            await _modal.HideModalAsync(cancellationToken);
        }

        private async Task Accept()
        {
            var xd = _editContext?.Validate();
            if (!_editContext?.Validate() ?? true)
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

        private async Task Cancel()
        {
            ClearModel();
            _tcs?.SetResult(null);
            await HideModalAsync();
        }

        public async Task InitializeDialogAsync(CategoryCreateDto model)
        {
            _model = model;
            _tcs = new TaskCompletionSource<CategoryCreateDto?>();
            _editContext = new EditContext(_model);

            await InvokeAsync(StateHasChanged);
        }

        public async Task<CategoryCreateDto?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null || _tcs is null)
                return null;

            await _modal.ShowModalAsync(cancellationToken);
            cancellationToken.Register(() => _tcs.TrySetCanceled());

            return await _tcs.Task;
        }
    }
}
