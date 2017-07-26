using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace HR.OAuth2.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizationServerAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            //1. If there are define allow Anonymous attribute, do nothing.
            if (IsDefinedAllowAnonymous(filterContext))
            {
                return;
            }

            //2. If there are no credentials, set the error result.
            var ticket = AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie).Result;

            var identity = ticket != null ? ticket.Identity : null;

            if (identity == null)
            {
                AuthenticationManager.Challenge(DefaultAuthenticationTypes.ApplicationCookie);

                filterContext.Result = new HttpUnauthorizedResult();

                return;
            }
            else
            {
                filterContext.Principal = new ClaimsPrincipal(identity);
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {

        }

        private bool IsDefinedAllowAnonymous(AuthenticationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            return filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
    }
}