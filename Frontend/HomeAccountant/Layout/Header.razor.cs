using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Layout
{
    public partial class Header : ComponentBase
    {
        [Parameter]
        public required string PageTitle { get; set; }
    }
}
