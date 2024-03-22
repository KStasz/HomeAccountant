using HomeAccountant.Core.Exceptions;
using Microsoft.AspNetCore.SignalR.Client;

namespace HomeAccountant.Core.Services
{
    public class SignalRHubConnection : ISignalRHubConnection
    {
        private const string HUB_IS_NOT_CONNECTED_EXCEPTION_MESSAGE = "Connection with hub hasn't been established";

        private readonly HubConnection _hubConnection;
        private readonly IHubConnectionSenderAsync _hubConnectionSenderAsync;
        private readonly IHubConnectionStateGetter _hubConnectionStateGetter;
        private readonly IHubConnectionConfigurator _hubConnectionConfigurator;

        public SignalRHubConnection(
            HubConnection hubConnection,
            IHubConnectionSenderAsync hubConnectionSenderAsync,
            IHubConnectionStateGetter hubConnectionStateGetter,
            IHubConnectionConfigurator hubConnectionConfigurator)
        {
            _hubConnection = hubConnection;
            _hubConnectionSenderAsync = hubConnectionSenderAsync;
            _hubConnectionStateGetter = hubConnectionStateGetter;
            _hubConnectionConfigurator = hubConnectionConfigurator;
        }

        public HubConnectionState State => _hubConnectionStateGetter.State;

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            return _hubConnection.StartAsync(cancellationToken);
        }

        public IDisposable On<TResult>(string? methodName, Func<TResult>? handler)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

            return _hubConnectionConfigurator.On(
                methodName,
                handler ?? throw new ArgumentNullException(nameof(handler)));
        }

        public IDisposable On<T1, TResult>(string? methodName, Func<T1, TResult>? handler)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

            return _hubConnectionConfigurator.On(
                methodName,
                handler ?? throw new ArgumentNullException(nameof(handler)));
        }

        public Task SendAsync(string? methodName, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

            if (State != HubConnectionState.Connected)
                throw new SignalRHubConnectionException(HUB_IS_NOT_CONNECTED_EXCEPTION_MESSAGE);

            return _hubConnectionSenderAsync.SendAsync(methodName, cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
