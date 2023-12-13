using HomeAccountant.Core.Services;
using Microsoft.AspNetCore.Components;

namespace HomeAccountant.Components.Alert
{
    public partial class Alert : ComponentBase, IAlert
    {
        [Inject]
        public required IJsCodeExecutor JsCodeExecutor { get; set; }

        private string AlertIdentifier = $"Alert_{Guid.NewGuid()}";

        private Dictionary<AlertType, string> AlertTypesDictionary = new Dictionary<AlertType, string>()
        {
            { AlertType.Primary, AlertType.Primary.ToString().ToLower() },
            { AlertType.Secondary, AlertType.Secondary.ToString().ToLower() },
            { AlertType.Success, AlertType.Success.ToString().ToLower() },
            { AlertType.Danger, AlertType.Danger.ToString().ToLower() },
            { AlertType.Warning, AlertType.Warning.ToString().ToLower() },
            { AlertType.Info, AlertType.Info.ToString().ToLower() },
            { AlertType.Light, AlertType.Light.ToString().ToLower() },
            { AlertType.Dark, AlertType.Dark.ToString().ToLower() }
        };

        public async Task ShowAlert(string message, AlertType type)
        {
            await JsCodeExecutor.ExecuteFunction(
                "ShowAlert",
                AlertIdentifier,
                AlertTypesDictionary[type],
                message);
        }
    }
}
