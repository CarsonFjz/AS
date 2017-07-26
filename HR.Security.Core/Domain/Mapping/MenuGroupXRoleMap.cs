namespace HR.Security.Core.Domain.Mapping
{
    public class MenuGroupXRoleMap : EntityTypeConfigurationBase<MenuGroupXRole>
    {
        public MenuGroupXRoleMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            // Table & Column Mappings
            this.ToTable("MenuGroupXRole");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastModifiedDate).HasColumnName("LastModifiedDate");
            this.Property(t => t.LastModifiedBy).HasColumnName("LastModifiedBy");
            this.Property(t => t.MenuGroupID).HasColumnName("MenuGroupID");
            this.Property(t => t.RoleID).HasColumnName("RoleID");

            // Relationships
            this.HasRequired(t => t.MenuGroup)
                .WithMany(t => t.MenuGroupXRoles)
                .HasForeignKey(d => d.MenuGroupID);
            this.HasRequired(t => t.Role)
                .WithMany(t => t.MenuGroupXRoles)
                .HasForeignKey(d => d.RoleID);

        }
    }
}
