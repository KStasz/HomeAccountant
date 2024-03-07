using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Components
{
    public partial class NullableTable<T> : ComponentBase
    {
        [Parameter]
        public ServiceResponse<IEnumerable<T>?>? Collection { get; set; }

        [Parameter]
        public RenderFragment<ServiceResponse<IEnumerable<T>?>>? ChildContent { get; set; }

        [Parameter]
        public IAlert? AlertReference { get; set; }

        private IAlert? InternalAlert;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(StateHasChanged);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
