using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IEntriesRealTimeService : IRealTimeDefaults
    {
        delegate Task EntryCreatedHandler(object sender, RealTimeEventArgs e);
        event EntryCreatedHandler? EntryCreated;
        Task EntryCreatedAsync(CancellationToken cancellationToken = default);
    }
}
