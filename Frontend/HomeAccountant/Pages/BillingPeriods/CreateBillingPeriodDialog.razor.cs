using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HomeAccountant.Pages.BillingPeriods
{
    public partial class CreateBillingPeriodDialog : ComponentBase, IModalDialog<BillingPeriodModel, BillingPeriodModel>
    {
        private IModal? _modal;
        private BillingPeriodModel? _model;
        private TaskCompletionSource<BillingPeriodModel?>? _tcs;
        private EditContext? _editContext;

        public Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null)
                return Task.CompletedTask;

            return _modal.HideModalAsync();
        }

        public async Task InitializeDialogAsync(BillingPeriodModel model)
        {
            _model = model;
            _tcs = new TaskCompletionSource<BillingPeriodModel?>();
            _editContext = new EditContext(_model);

            await InvokeAsync(StateHasChanged);
        }

        public async Task<BillingPeriodModel?> ShowModalAsync(CancellationToken cancellationToken = default)
        {
            if (_modal is null || _tcs is null)
                return null;

            await _modal.ShowModalAsync();

            return await _tcs.Task;
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

        private async Task Create()
        {
            if (!_editContext?.Validate() ?? true)
                return;

            _tcs?.SetResult(_model);
            await HideModalAsync();
        }
    }
}
