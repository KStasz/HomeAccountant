using Microsoft.EntityFrameworkCore;

namespace HomeAccountant.IdentityPlatform.Data
{
    public static class PrepDb
    {
        public static void PrepareDatabase(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (dbContext == null)
                {
                    Console.WriteLine($"--> Unable to apply migrations DbContext is null!");
                    
                    throw new ArgumentNullException(nameof(dbContext));
                }

                dbContext.Database.Migrate();
            }
        }
    }
}
