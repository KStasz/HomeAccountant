using Microsoft.EntityFrameworkCore;

namespace DomainTests.Mock
{
    public class InMemoryDatabaseService : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<SampleModelMock>()
                .HasOne(x => x.ReferencedSampleModelMock)
                .WithOne(x => x.SampleModelMock)
                .HasForeignKey<SampleModelMock>(x => x.ReferencedSampleModelMockId);

            modelBuilder
                .Entity<ReferencedSampleModelMock>()
                .HasOne(x => x.SampleModelMock)
                .WithOne(x => x.ReferencedSampleModelMock)
                .HasForeignKey<ReferencedSampleModelMock>(x => x.SampleModelMockId);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<SampleModelMock> SampleModel { get; set; }
        public DbSet<ReferencedSampleModelMock> ReferencedSampleModel { get; set; }
    }
}
