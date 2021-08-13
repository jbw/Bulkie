using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulkie.API.Infrastructure.EntityConfigurations
{
    class BulkieFileEntityTypeConfiguration : IEntityTypeConfiguration<Model.BulkieFile>
    {
        public void Configure(EntityTypeBuilder<Model.BulkieFile> bulkieEntityTypeConfiguration)
        {

            bulkieEntityTypeConfiguration.HasKey(ct => ct.Id);

            bulkieEntityTypeConfiguration.HasIndex(ct => ct.Status);

            bulkieEntityTypeConfiguration.Property(ct => ct.Status)
                .HasMaxLength(200)
                .IsRequired();

            bulkieEntityTypeConfiguration.HasOne(ct => ct.Bulkie);
        }
    }
}
