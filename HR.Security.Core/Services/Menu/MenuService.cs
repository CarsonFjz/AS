using HR.Security.Core.Data;
using HR.Security.Core.Domain;
using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using HR.Security.Core.Services.Users;

namespace HR.Security.Core.Services.Menu
{
    public partial class MenuService : IMenuService
    {
        private readonly IRepository<MenuGroup> _menuGroupRepository;
        private readonly IRepository<UserAccount> _userAccountRepository;
        private readonly IRepository<MenuGroupXRole> _menuGroupXRoleRepository;
        private readonly IRepository<RoleXUserAccount> _roleXUserAccountRepository;

        public MenuService(IRepository<MenuGroup> menuGroupRepository,
                           IRepository<UserAccount> userAccountRepository, 
                           IRepository<MenuGroupXRole> menuGroupXRoleRepository, 
                           IRepository<RoleXUserAccount> roleXUserAccountRepository)
        {            
            this._menuGroupRepository = menuGroupRepository;
            this._userAccountRepository = userAccountRepository;
            this._menuGroupXRoleRepository = menuGroupXRoleRepository;
            this._roleXUserAccountRepository = roleXUserAccountRepository;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public async Task<List<MenuGroup>> GetByUserIdAsync(Guid userId)
        {
            //1. 获取所有菜单。
            var menuGroups = await _menuGroupRepository.GetAllAsync();

            if(menuGroups != null && menuGroups.Count > 0)
            {
                //2. 首先把不需要角色的菜单添加。
                var resultMenu = menuGroups.Where(x => !x.AuthenticationRequire).ToList();

                var needAuthenticationMenu = menuGroups.Where(x => x.AuthenticationRequire).ToList();

                //3. 判断用户是否拥有菜单需要的角色。
                if (needAuthenticationMenu.Count > 0)
                {
                    var user = await _userAccountRepository.GetByIdAsync(userId);

                    //3.1 获取用户拥有的角色ID集合。
                    List<Guid> userRoleIds = await _roleXUserAccountRepository.Table.Where(x => x.UserAccountID == user.ID).Select(x => x.RoleID).ToListAsync();

                    foreach (var menuGroup in needAuthenticationMenu)
                    {
                        //3.2 获取该菜单需要的角色ID集合。
                        var menuGroupRoleIds = await _menuGroupXRoleRepository.Table.Where(x => x.MenuGroupID == menuGroup.ID).Select(x => x.RoleID).ToListAsync();

                        //3.3 判断用户拥有的角色是否包含菜单需要的角色。
                        bool isIncludeMenuRequireRoles = menuGroupRoleIds.All(m => userRoleIds.Any(u => u == m));

                        if (isIncludeMenuRequireRoles)
                        {
                            //3.4 如果包含菜单需要的角色则添加到菜单列表。
                            resultMenu.Add(menuGroup);
                        }                        
                    }
                }                
                
                return resultMenu.OrderBy(x => x.DisplayOrder).ToList();
            }

            return null;
        }
    }
}
