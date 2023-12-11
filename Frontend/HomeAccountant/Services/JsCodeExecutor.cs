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

        public async Task ExecuteFunction(string functionName)
        {
            await _jSRuntime.InvokeVoidAsync(functionName);
        }

        public async Task ExecuteFunction(string functionName, params object[] parameters)
        {
            await _jSRuntime.InvokeVoidAsync(functionName, parameters);
        }
    }
}
