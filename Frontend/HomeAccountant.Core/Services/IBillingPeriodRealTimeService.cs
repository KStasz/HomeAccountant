using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IBillingPeriodRealTimeService : IRealTimeDefaults
    {
        delegate Task EntryCreatedHandler(object sender, RealTimeEventArgs<int> e);
        event EntryCreatedHandler? EntryCreated;
        event EntryCreatedHandler? BillingPeriodStateChanged;

        Task EntryCreatedAsync(int billingPeriodId, CancellationToken cancellationToken = default);
        Task BillingPeriodStateChangedAsync(int billingPeriodId, CancellationToken cancellationToken = default);
    }
}
