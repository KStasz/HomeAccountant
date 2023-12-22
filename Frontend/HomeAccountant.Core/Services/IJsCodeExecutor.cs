namespace HomeAccountant.Core.Services
{
    public interface IJsCodeExecutor
    {
        Task ExecuteFunctionAsync(string functionName, CancellationToken cancellationToken = default);
        Task ExecuteFunctionAsync(string functionName, CancellationToken cancellationToken = default, params object[] parameters);
    }
}
