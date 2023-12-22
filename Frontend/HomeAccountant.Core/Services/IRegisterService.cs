using HomeAccountant.Core.DTOs.Register;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IRegisterService
    {
        Task<ServiceResponse<IEnumerable<RegisterReadDto>>> GetRegistersAsync(CancellationToken cancellationToken = default);
        Task<ServiceResponse> CreateRegisterAsync(RegisterCreateDto registerCreateDto, CancellationToken cancellationToken = default);
        Task<ServiceResponse> DeleteRegisterAsync(RegisterReadDto registerReadDto, CancellationToken cancellationToken = default);
    }
}
