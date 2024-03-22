namespace HomeAccountant.Core.Services
{
    public interface IHubConnectionSenderAsync
    {
        Task SendAsync(string? methodName, CancellationToken cancellationToken = default);
    }
}
