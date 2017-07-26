using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using HR.OAuth2.Models;
using HR.Security.Core.Services.Users;
using HR.Security.Core.Security;
using HR.Security.Core.Results;
using HR.Message.Contract.Event;
using HR.OAuth2.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace HR.OAuth2.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserAccountService _userAccountService;

        public AccountController(IUserAccountService userAccountService)
        {
            this._userAccountService = userAccountService;
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var resetPasswordStatus = await _userAccountService.ResetPasswordAsync(CurrentIdentityUserID, model.OldPassword, model.NewPassword);

                switch(resetPasswordStatus)
                {
                    // 如果修改成功，则需要重新登录。
                    case ResetPasswordResult.Succeeded:

                        ViewBag.IsSucceeded = true;

                        break;

                    case ResetPasswordResult.WrongPassword:

                        AddErrors("WrongPassword", "当前密码错误");

                        break;

                    case ResetPasswordResult.Failure:

                        AddErrors("修改失败");

                        break;

                    // 如果找不到用户，不要显示该用户不存在。
                    case ResetPasswordResult.NotFound:
                        break;
                }
            }

            return View();
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }

        //Just test environment.        
        //[AllowAnonymous]
        //public async Task<ActionResult> InitUserAccount()
        //{
        //    //List<Tuple<string, string>> tuple = new List<Tuple<string, string>>();

        //    //string[] strArray = System.IO.File.ReadAllLines(@"D:\新建文本文档.txt");

        //    //foreach (var str in strArray)
        //    //{
        //    //    Tuple<string, string> t = new Tuple<string, string>(str, str);

        //    //    tuple.Add(t);
        //    //}

        //    //var userAccounts = await _userAccountService.AddUserAccountsAsync(tuple);

        //    var userAccounts = await _userAccountService.GetAllAsync();

        //    //send command.
        //    if(userAccounts != null && userAccounts.Count > 0)
        //    {
        //        List<InitStaffsCommandEntity> commandEntities = new List<InitStaffsCommandEntity>();

        //        foreach (var userAccount in userAccounts)
        //        {
        //            InitStaffsCommandEntity entity = new InitStaffsCommandEntity()
        //            {
        //                UserAccountID = userAccount.ID,
        //                UserName = userAccount.UserName,
        //                CreatedBy = userAccount.CreatedBy,
        //                CreatedDate = userAccount.CreatedDate,
        //                LastModifiedBy = userAccount.LastModifiedBy,
        //                LastModifiedDate = userAccount.LastModifiedDate
        //            };

        //            commandEntities.Add(entity);
        //        }

        //        InitStaffsCommand command = new InitStaffsCommand(commandEntities);

        //        await _busControl.Publish(command);
        //    }

        //    return new EmptyResult();
        //}

        private void AddErrors(string key = "", params string[] errors)
        {
            foreach (string error in errors)
            {
                ModelState.AddModelError(key, error);
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