using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAccountant.Core.DTOs.Authentication
{
    public class LoginResponseDTO
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public bool Result { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
