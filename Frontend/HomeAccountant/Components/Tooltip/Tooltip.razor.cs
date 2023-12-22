using HomeAccountant.Core.Services;
using HomeAccountant.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Components.Tooltip
{
    public partial class Tooltip : ComponentBase
    {
        private string _tooltipIdentifier = $"Tooltip_{Guid.NewGuid()}";

        [Parameter]
        public required RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Text { get; set; } = string.Empty;

        [Parameter]
        public TooltipPlacement Placement { get; set; } = TooltipPlacement.Top;

        [Inject]
        public required IJsCodeExecutor JSCodeExecutor { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSCodeExecutor.ExecuteFunctionAsync("InitializeTooltip", default, _tooltipIdentifier);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private string TranslateTooltipPlacement()
        {
            switch (Placement)
            {
                case TooltipPlacement.Left:
                    return "left";
                case TooltipPlacement.Top:
                    return "top";
                case TooltipPlacement.Right:
                    return "right";
                case TooltipPlacement.Bottom:
                    return "bottom";
            }

            return "top";
        }
    }

    public enum TooltipPlacement
    {
        Left,
        Top,
        Right,
        Bottom
    }
}
