
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

        public async Task CloseModalAsync(string modalId, CancellationToken cancellationToken = default)
        {
            await _jsCodeExecutor.ExecuteFunctionAsync("HideModalWithIdentifier", cancellationToken, modalId);
        }

        public async Task ShowModalAsync(string modalId, CancellationToken cancellationToken = default)
        {
            await _jsCodeExecutor.ExecuteFunctionAsync("ShowModalWithIdentifier", cancellationToken, modalId);
        }

        public async Task InitializeModalAsync(string modalId, CancellationToken cancellationToken = default)
        {
            await _jsCodeExecutor.ExecuteFunctionAsync("Initialize", cancellationToken, modalId);
        }
    }
}
