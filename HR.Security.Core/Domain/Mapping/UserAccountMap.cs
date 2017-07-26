namespace HR.Security.Core.Domain.Mapping
{
    public class UserAccountMap : EntityTypeConfigurationBase<UserAccount>
    {
        public UserAccountMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.PasswordSalt)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("UserAccount");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastModifiedDate).HasColumnName("LastModifiedDate");
            this.Property(t => t.LastModifiedBy).HasColumnName("LastModifiedBy");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");            
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
