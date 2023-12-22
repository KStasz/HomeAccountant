using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class DbServiceConfigurationExtension
    {
        public static void ConfigureDbContext(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(DbContextRepository<,>));
        }
    }
}
