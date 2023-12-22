using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Data
{
    public static class DatabasePrep
    {
        public static void PrepareDatabase<T>(this IApplicationBuilder applicationBuilder) where T : DbContext
        {
            Console.WriteLine("--> Migration started...");
            try
            {
                using (var scope = applicationBuilder.ApplicationServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<T>();

                    dbContext.Database.Migrate();
                }

                Console.WriteLine("--> Migration completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Migration failed: {ex.Message}");
            }
        }
    }
}
