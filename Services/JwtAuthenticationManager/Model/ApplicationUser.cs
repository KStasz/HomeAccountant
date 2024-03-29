﻿using Microsoft.AspNetCore.Identity;

namespace JwtAuthenticationManager.Model;

public class ApplicationUser : IdentityUser
{
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
}
