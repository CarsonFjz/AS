using System;
using System.Collections.Generic;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security;
using HR.OAuth2.Providers;
using Microsoft.AspNet.Identity;
using HR.Security.Core.Settings;

namespace HR.OAuth2
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }
        
        public void ConfigureAuth(IAppBuilder app)
        {            
            // 针对基于 OAuth 的流配置应用程序
            PublicClientId = "self";

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                AuthenticationMode = AuthenticationMode.Passive,
                CookieName = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/sso/login"),
                LogoutPath = new PathString("/sso/logout"),
                ExpireTimeSpan = TimeSpan.FromDays(2),
                CookieDomain = AppSettingConfig.CookieDomain
            });

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                AuthorizeEndpointPath = new PathString("/OAuth2/Authorize"),

                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizationCodeProvider = new ApplicationAuthorizationCodeProvider(),
             
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(2),

                ApplicationCanDisplayErrors = true,
//#if DEBUG
//                AllowInsecureHttp = true,
//#endif
                AllowInsecureHttp = true
            };
            
            // 使应用程序可以使用不记名令牌来验证用户身份
            app.UseOAuthBearerTokens(OAuthOptions);            
        }
    }
}
