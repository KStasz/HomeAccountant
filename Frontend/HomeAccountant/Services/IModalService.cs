namespace HomeAccountant.Services
{
    public interface IModalService
    {
        Task CloseModalAsync(string modalId);
        Task InitializeModalAsync(string modalId);
        Task ShowModalAsync(string modalId);
    }
}
