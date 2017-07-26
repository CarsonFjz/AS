using System;
using System.Web.Mvc;

namespace HR.OAuth2.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}