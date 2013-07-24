using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Logging;

namespace eCentral.Data.Mapping.Logging
{
    public partial class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            this.ToTable("SystemLog");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(l => l.RowId);
            this.Property(l => l.ShortMessage).IsRequired().IsMaxLength();
            this.Property(l => l.FullMessage).IsMaxLength();
            this.Property(l => l.IpAddress).HasMaxLength(200);

            this.Ignore(l => l.LogLevel);

            this.HasOptional(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
            .WillCascadeOnDelete(true);
        }
    }
}