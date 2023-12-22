namespace HomeAccountant.Core.ViewModels
{
    public abstract class MvvmViewModel : BaseViewModel
    {
        public virtual Task PageInitializedAsync()
            => Task.CompletedTask;

        public virtual void PageInitialized() { }

        public virtual void PageParameterSet(Dictionary<string, object?> parameters) { }

        public virtual Task PageParameterSetAsync(Dictionary<string, object?> parameters)
            => Task.CompletedTask;

        public virtual void PageRendered(bool firstRender) { }

        public virtual Task PageRenderedAsync(bool firstRender)
            => Task.CompletedTask;

        public T? GetParameter<T>(object? value)
        {
            return (T?)Convert.ChangeType(value, typeof(T));
        }
    }
}
