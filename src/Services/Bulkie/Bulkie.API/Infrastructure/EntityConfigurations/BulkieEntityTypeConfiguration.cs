using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulkie.API.Infrastructure.EntityConfigurations
{
    class BulkieEntityTypeConfiguration : IEntityTypeConfiguration<Model.Bulkie>
    {
        public void Configure(EntityTypeBuilder<Model.Bulkie> bulkieEntityTypeConfiguration)
        {

            bulkieEntityTypeConfiguration.HasKey(ct => ct.Id);

            bulkieEntityTypeConfiguration.Property(ct => ct.Status)
                .HasMaxLength(200)
                .IsRequired();

            bulkieEntityTypeConfiguration
                .HasMany(ct => ct.BulkieFiles)
                .WithOne(ct => ct.Bulkie);
        }
    }
}
