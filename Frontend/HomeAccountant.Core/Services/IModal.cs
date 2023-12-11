namespace HomeAccountant.Core.Services
{
    public interface IModalDialog<T, U>
    {
        Task InitializeDialogAsync(T model);
        Task<U?> ShowModalAsync();
        Task HideModalAsync();
    }

    public interface IModalDialog<T>
    {
        Task InitializeDialogAsync(T model);
        Task<ModalResult> ShowModalAsync();
        Task HideModalAsync();
    }

    public interface IModal<T>
    {
        Task ShowModalAsync(T model);
        Task HideModalAsync();
    }

    public interface IModal
    {
        Task ShowModalAsync();
        Task HideModalAsync();
    }
}
