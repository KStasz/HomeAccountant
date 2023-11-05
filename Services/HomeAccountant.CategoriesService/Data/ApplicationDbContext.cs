using HomeAccountant.CategoriesService.Model;
using Microsoft.EntityFrameworkCore;

namespace HomeAccountant.CategoriesService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CategoryModel> Categories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<CategoryModel>()
                .HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
