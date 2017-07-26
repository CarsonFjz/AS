using System;
using System.Collections.Generic;

namespace HR.Security.Core.Domain
{
    public partial class Role : EntityBase
    {
        public Role()
        {
            this.MenuGroupXRoles = new List<MenuGroupXRole>();
            this.RoleXUserAccounts = new List<RoleXUserAccount>();
        }                
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<MenuGroupXRole> MenuGroupXRoles { get; set; }
        public virtual ICollection<RoleXUserAccount> RoleXUserAccounts { get; set; }
    }
}
