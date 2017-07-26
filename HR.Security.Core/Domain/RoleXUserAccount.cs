using System;

namespace HR.Security.Core.Domain
{
    public partial class RoleXUserAccount : EntityBase
   {
        public Guid RoleID { get; set; }
        public Guid UserAccountID { get; set; }
        public virtual Role Role { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
