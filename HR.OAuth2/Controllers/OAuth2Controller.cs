using HR.OAuth2.Models;
using HR.Security.Core.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HR.OAuth2.Controllers
{
    public class OAuth2Controller : BaseController
    {
        public ActionResult Authorize()
        {
            var ticket = AuthenticationManager.AuthenticateAsync(DefaultAuthenticationTypes.ApplicationCookie).Result;

            var identity = ticket != null ? ticket.Identity : null;

            if (identity == null)
            {
                AuthenticationManager.Challenge(DefaultAuthenticationTypes.ApplicationCookie);

                return new HttpUnauthorizedResult();
            }

            identity = new ClaimsIdentity(identity.Claims, "Bearer", identity.NameClaimType, identity.RoleClaimType);

            AuthenticationManager.SignIn(identity);

            return new EmptyResult();
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}