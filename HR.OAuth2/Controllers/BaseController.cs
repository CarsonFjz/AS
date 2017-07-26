using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace HR.OAuth2.Controllers
{
    public class BaseController : Controller
    {
        protected Guid CurrentIdentityUserID
        {
            get
            {
                Guid userId = Guid.Empty;

                Guid.TryParse(User.Identity.GetUserId(), out userId);

                return userId;
            }
        }
    }
}