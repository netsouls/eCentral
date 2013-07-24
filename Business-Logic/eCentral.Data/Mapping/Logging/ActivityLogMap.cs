using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Logging;

namespace eCentral.Data.Mapping.Logging
{
    public partial class ActivityLogMap : EntityTypeConfiguration<ActivityLog>
    {
        public ActivityLogMap()
        {
            this.ToTable("ActivityLog");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(al => al.RowId);
            this.Property(al => al.VersionControl).IsRequired().IsMaxLength();
            this.Property(al => al.Comments).IsRequired().IsMaxLength();

            this.HasRequired(al => al.ActivityLogType)
                .WithMany(alt => alt.ActivityLog)
                .HasForeignKey(al => al.ActivityLogTypeId);

            this.HasRequired(al => al.User)
                .WithMany(c => c.ActivityLog)
                .HasForeignKey(al => al.UserId);
        }
    }
}
