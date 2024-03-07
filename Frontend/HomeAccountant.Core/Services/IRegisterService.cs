using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IRegisterService
    {
        Task<ServiceResponse<RegisterModel?>> GetRegister(int registerId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<IEnumerable<RegisterModel>?>> GetRegistersAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse> CreateRegisterAsync(RegisterModel registerCreateDto, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteRegisterAsync(int registerId, CancellationToken cancellationToken = default);
        Task<ServiceResponse<IEnumerable<RegisterModel>?>> GetSharedRegisters(CancellationToken cancellationToken = default);
        Task<ServiceResponse> ShareRegisterAsync(int registerId, string userId, CancellationToken cancellationToken = default);
    }
}
