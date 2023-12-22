namespace HomeAccountant.Core.Services
{
    public interface IModalDialog<T, U>
    {
        Task InitializeDialogAsync(T model);
        Task<U?> ShowModalAsync(CancellationToken cancellationToken = default);
        Task HideModalAsync(CancellationToken cancellationToken = default);
    }

    public interface IModalDialog<T>
    {
        Task InitializeDialogAsync(T model);
        Task<ModalResult> ShowModalAsync(CancellationToken cancellationToken = default);
        Task HideModalAsync(CancellationToken cancellationToken = default);
    }

    public interface IModal<T>
    {
        Task ShowModalAsync(T model, CancellationToken cancellationToken = default);
        Task HideModalAsync(CancellationToken cancellationToken = default);
    }

    public interface IModal
    {
        Task ShowModalAsync(CancellationToken cancellationToken = default);
        Task HideModalAsync(CancellationToken cancellationToken = default);
    }
}
