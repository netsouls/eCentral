using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Media;

namespace eCentral.Data.Mapping.Media
{
    public partial class FileMetaDataMap : EntityTypeConfiguration<FileMetaData>
    {
        public FileMetaDataMap()
        {
            this.ToTable("FileMetaData");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(c => c.RowId);

            this.Property(p => p.BinaryData).IsMaxLength();
            this.Property(p => p.MimeType).IsRequired().HasMaxLength(40);
            this.Property(p => p.SeoFilename).HasMaxLength(300);
        }
    }
}