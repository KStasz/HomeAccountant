namespace HomeAccountant.Core.Services
{
    public class PubSubService : IPubSubService
    {
        public delegate Task MessageSenderEventHandlerAsync(object? sender, EventArgs e);
        public event MessageSenderEventHandlerAsync? MessageSender;

        public async Task Send(object? sender)
        {
            if (MessageSender is null)
                return;

            await MessageSender.Invoke(sender, EventArgs.Empty);
        }
    }
}
