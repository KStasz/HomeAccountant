namespace HomeAccountant.Core.Services
{
    public interface ISignalRHubConnection : IAsyncDisposable, IHubConnectionSenderAsync, IHubConnectionStateGetter, IHubConnectionConfigurator
    {
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}