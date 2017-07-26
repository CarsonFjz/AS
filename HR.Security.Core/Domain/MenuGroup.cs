using System.Collections.Generic;

namespace HR.Security.Core.Domain
{
    public partial class MenuGroup : EntityBase
    {
        public MenuGroup()
        {
            this.MenuGroupXRoles = new List<MenuGroupXRole>();
        }        
        public string URL { get; set; }
        public string DisplayName { get; set; }
        public string AltTag { get; set; }
        public bool AuthenticationRequire { get; set; }
        public int DisplayOrder { get; set; }
        public virtual ICollection<MenuGroupXRole> MenuGroupXRoles { get; set; }
    }
}
