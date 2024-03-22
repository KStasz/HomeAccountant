using Microsoft.AspNetCore.SignalR.Client;

namespace HomeAccountant.Core.Services
{
    public interface IHubConnectionStateGetter
    {
        public HubConnectionState State { get; }
    }
}
