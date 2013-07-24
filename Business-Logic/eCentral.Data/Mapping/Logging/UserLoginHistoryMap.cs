using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;
using eCentral.Core.Domain.Logging;

namespace eCentral.Data.Mapping.Logging
{
    public partial class UserLoginHistoryMap : EntityTypeConfiguration<UserLoginHistory>
    {
        public UserLoginHistoryMap()
        {
            this.ToTable("UserLoginHistory");
            this.Property(item => item.RowId).IsRequired()
                .HasColumnType("UniqueIdentifier").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasKey(ulh => ulh.RowId);
            
            this.HasRequired(ut => ut.User)
                .WithMany(u => u.UserLoginHistory)
                .HasForeignKey(u => u.UserId);
        }
    }
}
