using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Cms;

namespace eCentral.Data.Mapping.Cms
{
    public partial class WidgetMap : EntityTypeConfiguration<Widget>
    {
        public WidgetMap()
        {
            this.ToTable("Widget");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            
            this.HasKey(pv => pv.RowId);
            this.Property(pv => pv.PluginSystemName).IsRequired().IsMaxLength();

            this.Ignore(pv => pv.WidgetZone);
        }
    }
}