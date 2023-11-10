using HomeAccountant.FriendsService.Service;
using JwtAuthenticationManager;

namespace HomeAccountant.FriendsService.Config
{
    public static class HttpClientRegistrations
    {
        public static void RegisterHttpClientServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            RegisterIdentityPlatformService(services, configuration);
        }

        private static void RegisterIdentityPlatformService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IIdentityPlatformService, IdentityPlatformService>((serviceProvider, config) =>
            {
                config.BaseAddress = new Uri(configuration["IdentityPlatformBaseAddress"] ?? throw new Exception("IdentityPlatform Base Address is not provided"));
                
                config.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", GetAuthorizationToken(serviceProvider));
            });
        }

        private static string GetAuthorizationToken(IServiceProvider serviceProvider)
        {
            var token = serviceProvider.GetRequiredService<IAuthorizationTokenProvider>().GetToken();
            
            return token;
        }
    }
}
