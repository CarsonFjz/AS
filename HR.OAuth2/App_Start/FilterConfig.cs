using HR.OAuth2.Filters;
using System.Web;
using System.Web.Mvc;
using HR.Security.Core.Settings;

namespace HR.OAuth2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (AppSettingConfig.SslRequirement)
            {
                filters.Add(new RequireHttpsAttribute());
            }
            filters.Add(new AuthorizationServerAuthenticationAttribute());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
