﻿using HomeAccountant.Core.DTOs.Authentication;
using HomeAccountant.Core.Model;

namespace HomeAccountant.Core.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse<LoginResponseDTO>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<ServiceResponse<LoginResponseDTO>> RegisterAsync(string email, string username, string password, CancellationToken cancellationToken = default);
        Task<ServiceResponse<TokenAuthenticationModel>> RefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    }
}
