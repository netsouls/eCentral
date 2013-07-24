using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Companies;

namespace eCentral.Data.Mapping.Companies
{
    public partial class BranchOfficeMap : EntityTypeConfiguration<BranchOffice>
    {
        public BranchOfficeMap()
        {
            this.ToTable("BranchOffices");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(c => c.RowId);
            
            this.Property(c => c.BranchName).IsRequired().HasMaxLength(1000);
            
            this.HasOptional(c => c.Address)
                .WithMany()
                .HasForeignKey(c => c.AddressId).WillCascadeOnDelete(false);

            this.HasMany(c => c.AuditHistory)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("BranchOfficeAuditHistory");
                    m.MapLeftKey("OfficeId");
                    m.MapRightKey("ActivityLogId");
                }
                );
        }
    }
}