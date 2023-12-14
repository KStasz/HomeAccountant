using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Layout
{
    public partial class Header : ComponentBase
    {
        [Parameter]
        public required string PageTitle { get; set; }

        [Inject]
        public required IJsCodeExecutor JsCodeExecutor { get; set; }

        private string _togglerId = "navbarToggler";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsCodeExecutor.ExecuteFunction("InitializeNavbar", _togglerId);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
