namespace HomeAccountant.Services
{
    public interface IModalService
    {
        Task CloseModalAsync(string modalId, CancellationToken cancellationToken = default);
        Task InitializeModalAsync(string modalId, CancellationToken cancellationToken = default);
        Task ShowModalAsync(string modalId, CancellationToken cancellationToken = default);
    }
}
