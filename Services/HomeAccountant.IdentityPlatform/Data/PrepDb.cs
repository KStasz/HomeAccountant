using JwtAuthenticationManager.Data;
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
                var jwtDbContext = scope.ServiceProvider.GetRequiredService<JwtAuthenticationManagerDbContext>();

                if (dbContext == null)
                {   
                    throw new ArgumentNullException(nameof(dbContext));
                }

                if (jwtDbContext == null)
                {
                    throw new ArgumentNullException(nameof(jwtDbContext));
                }

                dbContext.Database.Migrate();
                jwtDbContext.Database.Migrate();
            }
        }
    }
}
