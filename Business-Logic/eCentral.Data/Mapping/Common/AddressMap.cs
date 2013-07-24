using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Common;

namespace eCentral.Data.Mapping.Common
{
    public partial class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            this.ToTable("Addresses");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(a => a.RowId);

            this.HasOptional(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId).WillCascadeOnDelete(false);

            this.HasOptional(a => a.StateProvince)
                .WithMany()
                .HasForeignKey(a => a.StateProvinceId).WillCascadeOnDelete(false);
        }
    }
}