using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManager
{
    public class AuthorizationTokenProvider : IAuthorizationTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorizationTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetToken()
        {
            return _httpContextAccessor?.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? string.Empty;
        }
    }
}
