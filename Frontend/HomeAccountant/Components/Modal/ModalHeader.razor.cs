using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Components.Modal
{
    public partial class ModalHeader : ComponentBase
    {
        [Parameter]
        public string ModalTitle { get; set; } = "Modal title";

        [CascadingParameter(Name = "HideModal")]
        public Func<Task>? HideModal { get; set; }

        private async Task HideModal_Clicked()
        {
            if (HideModal != null)
            {
                await HideModal.Invoke();
            }
        }
    }
}
