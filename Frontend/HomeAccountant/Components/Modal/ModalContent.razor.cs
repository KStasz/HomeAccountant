using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Components.Modal
{
    public partial class ModalContent : ComponentBase
    {
        [Parameter]
        public required RenderFragment ChildContent { get; set; }
    }
}
