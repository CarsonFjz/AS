using HR.Security.Core.Services.Users;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Principal;
using System.Security.Claims;

namespace HR.OAuth2.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizationServerAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IUserAccountService _userAccountService;
        private string[] roles { get; set; }

        private Dictionary<string, string[]> cacheDic = new Dictionary<string, string[]>();

        public AuthorizationServerAuthorizeAttribute(IUserAccountService userAccountService)
        {
            this._userAccountService = userAccountService;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("HttpContext");
            }
            if (roles != null)
            {
                Guid userId;

                Guid.TryParse(httpContext.User.Identity.GetUserId(), out userId);

                return _userAccountService.IsInRole(userId, roles);
            }

            return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
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

            var controllerDescriptor = filterContext.ActionDescriptor.ControllerDescriptor;
            var actionDescriptor = filterContext.ActionDescriptor;

            var cacheKey = controllerDescriptor.ControllerName + "." + actionDescriptor.ActionName;

            //persistent property
            roles = null;

            if (!cacheDic.ContainsKey(cacheKey))
            {
                var attrs = actionDescriptor.GetCustomAttributes(typeof(RequireRolesAttribute), false);
                if (attrs.Length == 1)
                {
                    roles = ((RequireRolesAttribute)attrs[0]).Roles;
                }
                else
                {
                    attrs = controllerDescriptor.GetCustomAttributes(typeof(RequireRolesAttribute), false);
                    if (attrs.Length == 1)
                    {
                        roles = ((RequireRolesAttribute)attrs[0]).Roles;
                    }
                }
                if (roles != null)
                {
                    cacheDic[cacheKey] = roles;
                }
            }
            else
            {
                roles = cacheDic[cacheKey];
            }

            base.OnAuthorization(filterContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }

        private bool IsDefinedAllowAnonymous(AuthorizationContext filterContext)
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