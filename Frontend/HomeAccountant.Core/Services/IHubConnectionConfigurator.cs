namespace HomeAccountant.Core.Services
{
    public interface IHubConnectionConfigurator
    {
        IDisposable On<TResult>(string? methodName, Func<TResult>? handler);
        IDisposable On<T1, TResult>(string? methodName, Func<T1, TResult>? handler);
    }
}