using Microsoft.EntityFrameworkCore;

namespace HomeAccountant.AccountingService.Data
{
    public static class DatabasePrep
    {
        public static void PrepareDatabase(this IApplicationBuilder applicationBuilder)
        {
            Console.WriteLine("--> Migration started...");
            try
            {
                using (var scope = applicationBuilder.ApplicationServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

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
