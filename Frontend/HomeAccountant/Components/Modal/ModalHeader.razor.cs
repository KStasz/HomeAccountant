using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Components.Modal
{
    public partial class ModalHeader : ComponentBase
    {
        [Parameter]
        public string ModalTitle { get; set; } = "Modal title";

        [CascadingParameter(Name = "Modal")]
        public Modal? Modal { get; set; }

        private async Task HideModal_Clicked()
        {
            if (Modal != null)
            {
                await Modal.HideModalAsync();
            }
        }
    }
}
