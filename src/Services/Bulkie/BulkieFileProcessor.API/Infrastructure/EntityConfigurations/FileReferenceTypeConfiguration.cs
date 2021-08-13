using BulkieFileProcessor.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bulkie.API.Infrastructure.EntityConfigurations
{
    class FileReferenceTypeConfiguration : IEntityTypeConfiguration<FileReference>
    {
        public void Configure(EntityTypeBuilder<FileReference> bulkieEntityTypeConfiguration)
        {

            bulkieEntityTypeConfiguration.HasKey(ct => ct.FileHash );

            bulkieEntityTypeConfiguration.Property(ct => ct.FileHash)
                .HasMaxLength(64)
                .IsRequired();

        }
    }
}
