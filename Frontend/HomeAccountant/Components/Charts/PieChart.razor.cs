using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.Text.Json;

namespace HomeAccountant.Components.Charts
{
    public partial class PieChart : ComponentBase
    {
        private string _chartIdentifier = $"Chart_{Guid.NewGuid()}";

        [Inject]
        public required IJsCodeExecutor JsCodeExecutor { get; set; }

        [Parameter]
        public IEnumerable<ChartDataset>? Dataset { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Dataset is null)
                return;

            var config = GetJsonConfiguration(Dataset);
            await JsCodeExecutor.ExecuteFunctionAsync("CreateChart", cancellationToken: default, _chartIdentifier, config);

            await base.OnAfterRenderAsync(firstRender);
        }

        private string GetJsonConfiguration(IEnumerable<ChartDataset> dataset)
        {
            PieChartConfiguration config = new PieChartConfiguration(dataset);
            return JsonSerializer.Serialize(config);
        }

        public async Task DestroyChart()
        {
            await JsCodeExecutor.ExecuteFunctionAsync("DestroyChart");
        }
    }
}
