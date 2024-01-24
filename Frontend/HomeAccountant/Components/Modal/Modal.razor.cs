using HomeAccountant.Core.Services;
using HomeAccountant.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Components.Modal
{
    public partial class Modal : ComponentBase, IModal
    {
        [Parameter]
        public required RenderFragment ChildContent { get; set; }

        [Inject]
        public required IModalService ModalService { get; set; }

        [Parameter]
        public string? AdditionalModalCssClass { get; set; } = "";

        [Parameter]
        public ModalPosition Position { get; set; } = ModalPosition.Standard;

        private bool _isVisible = false;
        public string ModalIdentifier { get; set; } = $"Modal_{Guid.NewGuid()}";

        private Dictionary<ModalPosition, string> _positionsDictionary = new Dictionary<ModalPosition, string>()
        {
            { ModalPosition.Standard, string.Empty },
            { ModalPosition.Centered, "modal-dialog-centered" }
        };

        protected override void OnInitialized()
        {   
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await ModalService.InitializeModalAsync(ModalIdentifier);
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task ShowModalAsync(CancellationToken cancellationToken = default)
        {
            _isVisible = true;
            await ModalService.ShowModalAsync(ModalIdentifier);
        }

        public Task HideModalAsync(CancellationToken cancellationToken = default)
        {
            _isVisible = false;
            return ModalService.CloseModalAsync(ModalIdentifier);
        }
    }
}
