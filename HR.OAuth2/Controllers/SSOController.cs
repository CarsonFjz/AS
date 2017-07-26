using HR.Message.Contract.Event;
using HR.OAuth2.Models;
using HR.Security.Core.Results;
using HR.Security.Core.Services.Users;
using HR.Security.Core.Settings;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SJ.OA.Framework.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HR.OAuth2.Controllers
{
    public class SSOController : BaseController
    {        
        private readonly IUserAccountService _userAccountService;

        public SSOController(IUserAccountService userAccountService)
        {            
            this._userAccountService = userAccountService;
        }
        
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var authenticateResult = await _userAccountService.AuthenticateAsync(model.UserName, model.Password);

                var signInStatus = authenticateResult.Item1;
                var userAccount = authenticateResult.Item2;

                switch (signInStatus)
                {
                    case SignInStatus.Succeeded:

                        var authentication = HttpContext.GetOwinContext().Authentication;

                        AuthenticationProperties properties = new AuthenticationProperties()
                        {
                            IsPersistent = true
                        };

                        Claim nameClaim = new Claim(ClaimTypes.Name, userAccount.UserName);
                        Claim nameIdentifierClaim = new Claim(ClaimTypes.NameIdentifier, userAccount.ID.ToString());

                        ClaimsIdentity identity = new ClaimsIdentity
                        (
                            new[] { nameClaim, nameIdentifierClaim },

                            DefaultAuthenticationTypes.ApplicationCookie
                        );

                        authentication.SignIn(properties, identity);

                        return Redirect(returnUrl);

                    case SignInStatus.LocketOut:

                        AddErrors("该帐号已被锁定");

                        break;

                    case SignInStatus.WrongPassword:

                        AddErrors("密码错误");

                        break;

                    case SignInStatus.Failure:

                        AddErrors("不存在该帐号");

                        break;
                }
            }
            
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }

        public ActionResult LogOff()
        {
            //1. drop auth cookie.
            PassportAuthentication.DropAuthCookie();

            //2. sign out.
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            string urlReferrer = string.Empty;

            if (Request.UrlReferrer == null)
            {
                urlReferrer = AppSettingConfig.DefaultReturnUrl;
            }
            else
            {
                urlReferrer = Request.UrlReferrer.AbsoluteUri;

                //3. if the urlReferrer is local url, then redirect default url.
                if (Url.IsLocalUrl(Request.UrlReferrer.AbsolutePath))
                {
                    urlReferrer = AppSettingConfig.DefaultReturnUrl;
                }
            }

            return Redirect(urlReferrer);
        }

        private void AddErrors(params string[] errors)
        {
            foreach (string error in errors)
            {
                ModelState.AddModelError("", error);
            }
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