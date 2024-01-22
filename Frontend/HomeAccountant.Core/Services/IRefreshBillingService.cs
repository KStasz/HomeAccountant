
namespace HomeAccountant.Core.Services
{
    public interface IRefreshBillingService
    {
        Task RefreshBillingPeriodAsync(CancellationToken cancellationToken = default);
    }
}
