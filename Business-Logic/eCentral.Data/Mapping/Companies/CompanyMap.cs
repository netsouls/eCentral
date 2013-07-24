using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Companies;

namespace eCentral.Data.Mapping.Companies
{
    public partial class CompanyMap : EntityTypeConfiguration<Company>
    {
        public CompanyMap()
        {
            this.ToTable("Companies");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(c => c.RowId);
            
            this.Property(c => c.CompanyName).IsRequired().HasMaxLength(1000);
            this.Property(c => c.Abbreviation).IsRequired().HasMaxLength(10);

            this.HasOptional(c => c.Logo)
                .WithMany()
                .HasForeignKey(c => c.LogoId).WillCascadeOnDelete(false);

            this.HasMany(c => c.AuditHistory)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("CompanyAuditHistory");
                    m.MapLeftKey("CompanyId");
                    m.MapRightKey("ActivityLogId");
                }
            );
        }
    }
}