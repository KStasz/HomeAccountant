using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.Text.Json;

namespace HomeAccountant.Components.Charts
{
    public partial class PieChart : ComponentBase, IChart
    {
        private string _chartIdentifier = $"Chart_{Guid.NewGuid()}";

        [Inject]
        public required IJsCodeExecutor JsCodeExecutor { get; set; }

        [Parameter]
        public IEnumerable<ChartDataset>? Dataset { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Dataset is not null)
            {
                var config = GetJsonConfiguration(Dataset);
                await JsCodeExecutor.ExecuteFunction("CreateChart", _chartIdentifier, config);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task CreateChart(IEnumerable<ChartDataset> dataset)
        {
            var config = GetJsonConfiguration(dataset);
            await JsCodeExecutor.ExecuteFunction("CreateChart", _chartIdentifier, config);
        }

        private string GetJsonConfiguration(IEnumerable<ChartDataset> dataset)
        {
            PieChartConfiguration config = new PieChartConfiguration(dataset);
            return JsonSerializer.Serialize(config);
        }
    }
}
