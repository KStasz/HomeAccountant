using HomeAccountant.FriendsService.Model;
using Microsoft.EntityFrameworkCore;

namespace HomeAccountant.FriendsService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Friendships> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Friendships>()
                .Property(x => x.CreationDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder
                .Entity<Friendships>()
                .Property(x => x.IsActive)
                .HasDefaultValueSql("1");

            modelBuilder
                .Entity<Friendships>()
                .Property(x => x.IsAccepted)
                .HasDefaultValueSql("0");
        }
    }
}
