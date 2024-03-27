namespace HomeAccountant.Core.Services
{
    public interface IHubConnectionSenderAsync
    {
        Task SendAsync(string? methodName, CancellationToken cancellationToken = default);
        Task SendAsync(string? methodName, object? arg1, CancellationToken cancellationToken = default);
    }
}
