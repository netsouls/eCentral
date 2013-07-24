using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Users;

namespace eCentral.Data.Mapping.Users
{
    public partial class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("Users");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(c => c.RowId);
            this.Property(u => u.Username).HasMaxLength(1000);
            this.Property(u => u.Password);

            this.HasOptional(c => c.Language)
                .WithMany()
                .HasForeignKey(c => c.LanguageId).WillCascadeOnDelete(false);

            this.HasOptional(c => c.Currency)
                .WithMany()
                .HasForeignKey(c => c.CurrencyId).WillCascadeOnDelete(false);

            this.HasMany(c => c.UserRoles)
                .WithMany()
                //.Map(m => m.ToTable("UsersInRoles").MapLeftKey(u=>u.RowId,"UserId"));
                .Map(m =>
                    {
                        m.ToTable("UsersInRoles");
                        m.MapLeftKey("UserId");
                        m.MapRightKey("RoleId");
                    }
                );

            this.HasMany(c => c.AuditHistory)
                .WithMany()
                .Map(m =>
                {
                    m.ToTable("UserAuditHistory");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("ActivityLogId");
                }
                );
        }
    }
}