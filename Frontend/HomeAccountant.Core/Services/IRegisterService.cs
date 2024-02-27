using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IRegisterService
    {
        Task<ServiceResponse<IEnumerable<RegisterModel>?>> GetRegistersAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse> CreateRegisterAsync(RegisterModel registerCreateDto, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteRegisterAsync(int registerId, CancellationToken cancellationToken = default);
    }
}
