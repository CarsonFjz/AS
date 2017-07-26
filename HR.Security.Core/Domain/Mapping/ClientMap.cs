using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Security.Core.Domain.Mapping
{
    public class ClientMap : EntityTypeConfigurationBase<Client>
    {
        public ClientMap()
        {
            // Primary Key
            this.HasKey(t => t.ClientID);

            this.Property(t => t.ClientID)
                .IsRequired()
                .HasMaxLength(50);

            // Properties
            this.Property(t => t.ClientSecret)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.RedirectUri)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Client");
            this.Property(t => t.ClientID).HasColumnName("ClientID");
            this.Property(t => t.ClientSecret).HasColumnName("ClientSecret");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.RedirectUri).HasColumnName("RedirectUri");
            this.Property(t => t.Enabled).HasColumnName("Enabled");
        }
    }
}
