using HomeAccountant.Core.Exceptions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace HomeAccountant.Core.Services
{
    public class SignalRHubConnection : ISignalRHubConnection
    {
        private const string HUB_IS_NOT_CONNECTED_EXCEPTION_MESSAGE = "Connection with hub hasn't been established";
        private const string STATE_PATTERN = "{state}";
        private const string CONNECTION_STATE_MESSAGE = $"Connection state: {STATE_PATTERN}";

        private readonly HubConnection _hubConnection;
        private readonly IHubConnectionSenderAsync _hubConnectionSenderAsync;
        private readonly IHubConnectionStateGetter _hubConnectionStateGetter;
        private readonly IHubConnectionConfigurator _hubConnectionConfigurator;
        private readonly ILogger<SignalRHubConnection> _logger;

        public SignalRHubConnection(
            HubConnection hubConnection,
            IHubConnectionSenderAsync hubConnectionSenderAsync,
            IHubConnectionStateGetter hubConnectionStateGetter,
            IHubConnectionConfigurator hubConnectionConfigurator,
            ILogger<SignalRHubConnection> logger)
        {
            _hubConnection = hubConnection;
            _hubConnectionSenderAsync = hubConnectionSenderAsync;
            _hubConnectionStateGetter = hubConnectionStateGetter;
            _hubConnectionConfigurator = hubConnectionConfigurator;
            _logger = logger;
        }

        public HubConnectionState State => _hubConnectionStateGetter.State;

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _hubConnection.StartAsync(cancellationToken);
            _logger.LogInformation(CONNECTION_STATE_MESSAGE.Replace(STATE_PATTERN, State.ToString()));
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
            _logger.LogInformation(CONNECTION_STATE_MESSAGE.Replace(STATE_PATTERN, State.ToString()));
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
