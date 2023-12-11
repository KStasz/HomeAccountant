
using HomeAccountant.Core.Services;

namespace HomeAccountant.Services
{
    public class ModalService : IModalService
    {
        private readonly IJsCodeExecutor _jsCodeExecutor;

        public ModalService(IJsCodeExecutor jsCodeExecutor)
        {
            _jsCodeExecutor = jsCodeExecutor;
        }

        public async Task CloseModalAsync(string modalId)
        {
            await _jsCodeExecutor.ExecuteFunction("HideModalWithIdentifier", modalId);
        }

        public async Task ShowModalAsync(string modalId)
        {
            await _jsCodeExecutor.ExecuteFunction("ShowModalWithIdentifier", modalId);
        }

        public async Task InitializeModalAsync(string modalId)
        {
            await _jsCodeExecutor.ExecuteFunction("Initialize", modalId);
        }
    }
}
