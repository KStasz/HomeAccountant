namespace HomeAccountant.Core.Services
{
    public interface IJsCodeExecutor
    {
        Task ExecuteFunction(string functionName);
        Task ExecuteFunction(string functionName, params object[] parameters);
    }
}
