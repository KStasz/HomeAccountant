namespace HomeAccountant.Core.Services
{
    public interface IRealTimeDefaults : IAsyncDisposable
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}