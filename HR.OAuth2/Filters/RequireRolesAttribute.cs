using System;

namespace HR.OAuth2.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]    
    public class RequireRolesAttribute : Attribute
    {
        public string[] Roles { get; set; }

        public RequireRolesAttribute(params string[] roles)
        {
            this.Roles = roles;
        }
    }
}