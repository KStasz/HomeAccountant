using HomeAccountant.Core.DTOs;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IRegisterService
    {
        Task<ServiceResponse<IEnumerable<RegisterReadDto>>> GetRegistersAsync();
        Task<ServiceResponse> CreateRegisterAsync(RegisterCreateDto registerCreateDto);
        Task<ServiceResponse> DeleteRegisterAsync(RegisterReadDto registerReadDto);
    }
}
