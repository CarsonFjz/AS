using HR.Security.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.Security.Core.Services.Menu
{
    public partial interface IMenuService
    {
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<List<MenuGroup>> GetByUserIdAsync(Guid userId);
    }
}
