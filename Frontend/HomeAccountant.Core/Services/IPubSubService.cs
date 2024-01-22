
namespace HomeAccountant.Core.Services
{
    public interface IPubSubService
    {
        event PubSubService.MessageSenderEventHandlerAsync? MessageSender;
        Task Send(object? sender);
    }
}