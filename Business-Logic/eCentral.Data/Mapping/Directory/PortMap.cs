using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Directory;

namespace eCentral.Data.Mapping.Directory
{
    public partial class PortMap : EntityTypeConfiguration<Port>
    {
        public PortMap()
        {
            this.ToTable("Port");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(sp => sp.RowId);
            this.Property(sp => sp.Name).IsRequired().HasMaxLength(100);
            this.Property(sp => sp.Abbreviation).HasMaxLength(100);

            this.HasRequired(sp => sp.Country)
                .WithMany(c => c.Ports)
                .HasForeignKey(sp => sp.CountryId);
        }
    }
}