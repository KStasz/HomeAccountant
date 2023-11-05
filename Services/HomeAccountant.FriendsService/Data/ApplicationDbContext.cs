using HomeAccountant.FriendsService.Model;
using Microsoft.EntityFrameworkCore;

namespace HomeAccountant.FriendsService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Friend> Friends { get; set; }

        public DbSet<FriendRequest> FriendRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Friend>()
                .Property(x => x.CreationDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder
                .Entity<Friend>()
                .Property(x => x.IsActive)
                .HasDefaultValueSql("1");

            modelBuilder
                .Entity<FriendRequest>()
                .Property(x => x.CreationDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder
                .Entity<FriendRequest>()
                .Property(x => x.IsActive)
                .HasDefaultValueSql("1");
        }
    }
}
