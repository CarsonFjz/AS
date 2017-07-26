using System;
using System.Collections.Generic;

namespace HR.Security.Core.Domain
{
    public partial class UserAccount : EntityBase
    {
        public UserAccount()
        {
            this.RoleXUserAccounts = new List<RoleXUserAccount>();            
        }        
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Status { get; set; }
        public virtual ICollection<RoleXUserAccount> RoleXUserAccounts { get; set; }        

        public const string STATUS_ACTIVE = "Active";
        public const string STATUS_SUSPENDED = "Suspend";
    }
}
