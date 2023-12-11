using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Components.Modal
{
    public partial class ModalBody : ComponentBase
    {
        [Parameter]
        public required RenderFragment ChildContent { get; set; }
    }
}
