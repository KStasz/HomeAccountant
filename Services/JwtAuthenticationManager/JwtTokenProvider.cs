using JwtAuthenticationManager.Config;
using JwtAuthenticationManager.Data;
using JwtAuthenticationManager.Exceptions;
using JwtAuthenticationManager.Extensions;
using JwtAuthenticationManager.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthenticationManager
{
    public class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly JwtConfig _jwtConfig;
        private const string JwtUserIdClaimType = "UserId";

        public JwtTokenProvider(TokenValidationParameters tokenValidationParameters,
            ApplicationDbContext applicationDbContext,
            IOptions<JwtConfig> options)
        {
            _tokenValidationParameters = tokenValidationParameters;
            _applicationDbContext = applicationDbContext;
            _jwtConfig = options.Value;
        }

        public async Task<JwtTokenPair?> VerifyAndGenerateToken(string token, string refreshToken)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false; //for testing

                var tokenInVerification = jwtTokenHandler.ValidateToken(token,
                    _tokenValidationParameters,
                    out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtSecurityToken)
                    return null;

                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);

                if (result == false)
                    return null;

                var utcExpiryDate = long.Parse(tokenInVerification
                    .Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value ?? throw new ArgumentNullException(nameof(JwtRegisteredClaimNames.Exp)));

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.Now)
                    throw new TokenVerificationException("Expired token");

                var storedToken = await _applicationDbContext.RefreshTokens.Include(x => x.User).FirstOrDefaultAsync(x => x.Token == refreshToken);

                if (storedToken == null)
                    throw new TokenVerificationException("Invalid tokens");

                if (storedToken.IsUsed)
                    throw new TokenVerificationException("Invalid tokens");

                if (storedToken.IsRevoked)
                    throw new TokenVerificationException("Invalid tokens");

                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (jti is null || storedToken.JwtId != jti)
                    throw new TokenVerificationException("Invalid tokens");

                if (storedToken.ExpiryDate >= DateTime.UtcNow)
                    throw new TokenVerificationException("Expired token");

                storedToken.IsUsed = true;

                _applicationDbContext.RefreshTokens.Update(storedToken);
                await _applicationDbContext.SaveChangesAsync();

                var newToken = await GenerateJwtToken(storedToken.User);

                return new JwtTokenPair(token: newToken.Token, refreshToken: newToken.RefreshToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw new TokenVerificationException("Expired token");
            }
        }

        private DateTime UnixTimeStampToDateTime(long utcExpiryDate)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal.AddSeconds(utcExpiryDate).ToUniversalTime();

            return dateTimeVal;
        }

        public async Task<JwtTokenPair> GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(JwtUserIdClaimType, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Email, value: user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),

                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpiresMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                Token = StringExtensions.Random(23),
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id
            };

            _applicationDbContext.RefreshTokens.Add(refreshToken);
            await _applicationDbContext.SaveChangesAsync();

            return new JwtTokenPair(jwtToken, refreshToken.Token);
        }
    }
}
