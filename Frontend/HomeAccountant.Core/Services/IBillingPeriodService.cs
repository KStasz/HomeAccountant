using HomeAccountant.Core.DTOs.BillingPeriod;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IBillingPeriodService
    {
        Task<ServiceResponse<IEnumerable<BillingPeriodReadDto>>> GetBiilingPeriodsAsync(int registerId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<BillingPeriodStatisticDto>> GetBillingPeriodStatisticAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default);
        Task<ServiceResponse> CreateBillingPeriodAsync(int registerId, BillingPeriodCreateDto createDto, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteBillingPeriodAsync(int registerId, int billindPeriodId, CancellationToken cancellationToken = default);
        Task<ServiceResponse> OpenBillingPeriodAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default);
        Task<ServiceResponse> CloseBillingPeriodAsync(int registerId, int billingPeriodId, CancellationToken cancellationToken = default);
    }
}
