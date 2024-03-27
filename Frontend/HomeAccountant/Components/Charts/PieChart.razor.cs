using HomeAccountant.Core.Interfaces;
using HomeAccountant.Core.Model;
using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;
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

        [Parameter]
        public INotifyPropertyChangedAsync? PropertyChangedAsync { get; set; }

        protected override void OnInitialized()
        {
            if (PropertyChangedAsync is null)
                return;

            PropertyChangedAsync.PropertyChangedAsync += PropertyChanged;
            base.OnInitialized();
        }

        private async Task PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            await UpdateChart();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Dataset is null)
                    return;

                var config = GetJsonConfiguration(Dataset);
                await JsCodeExecutor.ExecuteFunctionAsync("CreateChart", cancellationToken: default, _chartIdentifier, config);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private string GetJsonConfiguration(IEnumerable<ChartDataset> dataset)
        {
            PieChartConfiguration config = new PieChartConfiguration(dataset);
            return JsonSerializer.Serialize(config);
        }

        private async Task UpdateChart()
        {
            if (Dataset is null)
                return;

            var config = GetJsonConfiguration(Dataset);
            await JsCodeExecutor.ExecuteFunctionAsync("UpdateData", cancellationToken: default, config);
        }

        public async Task DestroyChart()
        {
            await JsCodeExecutor.ExecuteFunctionAsync("DestroyChart");
        }
    }
}
