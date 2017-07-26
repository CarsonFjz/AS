using System;

namespace HR.Security.Core.Domain
{
    public partial class MenuGroupXRole : EntityBase
    {        
        public Guid MenuGroupID { get; set; }
        public Guid RoleID { get; set; }
        public virtual MenuGroup MenuGroup { get; set; }
        public virtual Role Role { get; set; }
    }
}
