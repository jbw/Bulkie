using Bulkie.API.Infrastructure.EntityConfigurations;
using BulkieFileProcessor.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Bulkie.API.Infrastructure
{

    public class FileReferenceContext : DbContext
    {
        public const string DEFAULT_SCHEMA = "filereference";

        public DbSet<FileReference> FileReferences { get; set; }

        public FileReferenceContext(DbContextOptions<FileReferenceContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FileReferenceTypeConfiguration());
        }

    }
}
