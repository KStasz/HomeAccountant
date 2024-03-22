using Microsoft.AspNetCore.SignalR.Client;

namespace HomeAccountant.Core.Services
{
    public class HubConnectionConfigurator : IHubConnectionConfigurator
    {
        private readonly HubConnection _hubConnection;

        public HubConnectionConfigurator(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public IDisposable On<TResult>(string? methodName, Func<TResult>? handler)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

            return _hubConnection.On(
                methodName, 
                handler ?? throw new ArgumentNullException(nameof(handler)));
        }

        public IDisposable On<T1, TResult>(string? methodName, Func<T1, TResult>? handler)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

            return _hubConnection.On(
                methodName, 
                handler ?? throw new ArgumentNullException(nameof(handler)));
        }
    }
}
