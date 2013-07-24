using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Tasks;

namespace eCentral.Data.Mapping.Tasks
{
    public partial class ScheduleTaskMap : EntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            this.ToTable("ScheduleTask");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(t => t.RowId);
            this.Property(t => t.Name).IsRequired();
            this.Property(t => t.Type).IsRequired();
            this.Property(t => t.LastStarDate).IsOptional();
            this.Property(t => t.LastEndDate).IsOptional();
            this.Property(t => t.LastSuccessDate).IsOptional();
        }
    }
}