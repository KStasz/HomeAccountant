using HomeAccountant.AccountingService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeAccountant.AccountingService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Register> Registers { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<RegisterSharing> RegisterSharings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Register>()
                .Property(x => x.IsActive)
                .HasDefaultValueSql("1");

            modelBuilder
                .Entity<Register>()
                .Property(x => x.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<Entry>()
                .Property(x => x.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<Entry>()
                .HasOne(x => x.BillingPeriod)
                .WithMany(x => x.Entries)
                .HasForeignKey(x => x.BillingPeriodId);

            modelBuilder
                .Entity<RegisterSharing>()
                .Property(x => x.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder
                .Entity<RegisterSharing>()
                .HasOne(x => x.Register)
                .WithMany(x => x.Sharings)
                .HasForeignKey(x => x.RegisterId);

            modelBuilder
                .Entity<BillingPeriod>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<BillingPeriod>()
                .HasOne(x => x.Register)
                .WithMany(x => x.BillingPeriods)
                .HasForeignKey(x => x.RegisterId);

            modelBuilder.Entity<BillingPeriod>()
                .Property(x => x.CreationDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<BillingPeriod>()
                .Property(x => x.IsOpen)
                .HasDefaultValueSql("1");
        }
    }
}
