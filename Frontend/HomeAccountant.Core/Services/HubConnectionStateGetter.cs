using Microsoft.AspNetCore.SignalR.Client;

namespace HomeAccountant.Core.Services
{
    internal class HubConnectionStateGetter : IHubConnectionStateGetter
    {
        private readonly HubConnection _hubConnection;

        public HubConnectionStateGetter(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public HubConnectionState State => _hubConnection.State;
    }
}
