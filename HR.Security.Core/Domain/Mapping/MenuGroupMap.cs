namespace HR.Security.Core.Domain.Mapping
{
    public class MenuGroupMap : EntityTypeConfigurationBase<MenuGroup>
    {
        public MenuGroupMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.URL)
                .HasMaxLength(1000);

            this.Property(t => t.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AltTag)
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("MenuGroup");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            this.Property(t => t.LastModifiedDate).HasColumnName("LastModifiedDate");
            this.Property(t => t.LastModifiedBy).HasColumnName("LastModifiedBy");
            this.Property(t => t.URL).HasColumnName("URL");
            this.Property(t => t.DisplayName).HasColumnName("DisplayName");
            this.Property(t => t.AltTag).HasColumnName("AltTag");
            this.Property(t => t.AuthenticationRequire).HasColumnName("AuthenticationRequire");
            this.Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
        }
    }
}
