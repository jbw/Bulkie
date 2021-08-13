using Bulkie.API.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Bulkie.API.Infrastructure
{

    public class BulkieContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "bulkie";

        public DbSet<Model.Bulkie> Bulkies { get; set; }
        public DbSet<Model.BulkieFile> BulkieFiles { get; set; }

        public BulkieContext(DbContextOptions<BulkieContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new BulkieEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BulkieFileEntityTypeConfiguration());
        }
    }
}
