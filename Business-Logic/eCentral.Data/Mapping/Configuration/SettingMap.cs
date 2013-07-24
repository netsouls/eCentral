using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Configuration;

namespace eCentral.Data.Mapping.Configuration
{
    public partial class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            this.ToTable("ConfigurationSetting");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(s => s.RowId);
            this.Property(s => s.Name).IsRequired().HasMaxLength(200);
            this.Property(s => s.Value).IsRequired().HasMaxLength(2000);
        }
    }
}