using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using JwtAuthenticationManager.Config;
using System.Text;
using JwtAuthenticationManager.Data;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationManager
{
    public static class JwtAuthenticationConfigurationExtension
    {
        public static void AddJwtAuthentication(this IServiceCollection services)
        {
            IConfigurationRoot config = AddConfiguration();

            services.Configure<JwtConfig>(config.GetSection("JwtConfig"));

            services.AddDbContext<JwtAuthenticationManagerDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("SqlServer"));
            });

            var key = Encoding.ASCII.GetBytes(config.GetSection("JwtConfig:Secret").Value ?? throw new ArgumentNullException("Missing Jwt SecretKey"));

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false, //for dev
                ValidateAudience = false, //for dev
                RequireExpirationTime = false, //for dev
                ValidateLifetime = true
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt =>
            {
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddSingleton(tokenValidationParameters);
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        }

        private static IConfigurationRoot AddConfiguration()
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("jwtauthenticationsettings.json")
                .AddEnvironmentVariables();

            return configBuilder.Build();
        }
    }
}
