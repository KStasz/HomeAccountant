using HomeAccountant.Core.Services;
using Microsoft.JSInterop;

namespace HomeAccountant.Services
{
    public class JsCodeExecutor : IJsCodeExecutor
    {
        private readonly IJSRuntime _jSRuntime;

        public JsCodeExecutor(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task ExecuteFunctionAsync(string functionName, CancellationToken cancellationToken = default)
        {
            await _jSRuntime.InvokeVoidAsync(functionName);
        }

        public async Task ExecuteFunctionAsync(string functionName, CancellationToken cancellationToken = default, params object[] parameters)
        {
            await _jSRuntime.InvokeVoidAsync(functionName, parameters);
        }
    }
}
