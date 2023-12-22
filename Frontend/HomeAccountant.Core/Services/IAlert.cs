namespace HomeAccountant.Core.Services
{
    public interface IAlert
    {
        Task ShowAlertAsync(string message, AlertType type, CancellationToken cancellationToken = default);
    }
}
