using HomeAccountant.Core.DTOs.Category;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Pages
{
    public partial class CategoryDeleteModal : ComponentBase, IModalDialog<CategoryReadDto>
    {
        private CategoryReadDto? _model;
        private TaskCompletionSource<ModalResult>? _tcs;
        private IModal? _modal;

        public async Task HideModalAsync()
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

        public async Task InitializeDialogAsync(CategoryReadDto model)
        {
            _model = model;
            _tcs = new TaskCompletionSource<ModalResult>();

            await InvokeAsync(StateHasChanged);
        }

        public async Task<ModalResult> ShowModalAsync()
        {
            if (_modal is null || _tcs is null)
                return ModalResult.Cancel;

            await _modal.ShowModalAsync();
            return await _tcs.Task;
        }
    }
}
