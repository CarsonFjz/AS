using HR.Security.Core.Services.Menu;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HR.OAuth2.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            this._menuService = menuService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _menuService.GetByUserIdAsync(CurrentIdentityUserID);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}