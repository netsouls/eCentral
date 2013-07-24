using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Clients;

namespace eCentral.Data.Mapping.Clients
{
    public partial class ClientMap : EntityTypeConfiguration<Client>
    {
        public ClientMap()
        {
            this.ToTable("Clients");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(c => c.RowId);
            
            this.Property(c => c.ClientName).IsRequired().HasMaxLength(1000);
            this.Property(c => c.Email).IsRequired().HasMaxLength(1000);

            this.HasOptional(c => c.Address)
                .WithMany()
                .HasForeignKey(c => c.AddressId).WillCascadeOnDelete(false);

            this.HasMany(c => c.AuditHistory)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("ClientAuditHistory");
                    m.MapLeftKey("ClientId");
                    m.MapRightKey("ActivityLogId");
                }
                );
        }
    }
}