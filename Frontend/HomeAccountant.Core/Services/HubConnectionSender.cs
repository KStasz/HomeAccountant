using Microsoft.AspNetCore.SignalR.Client;

namespace HomeAccountant.Core.Services
{
    public class HubConnectionSender : IHubConnectionSenderAsync
    {
        private readonly HubConnection _hubConnection;

        public HubConnectionSender(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public Task SendAsync(string? methodName, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

            return _hubConnection.SendAsync(methodName, cancellationToken);
        }
    }
}
