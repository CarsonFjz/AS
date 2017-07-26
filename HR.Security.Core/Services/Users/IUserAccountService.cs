using HR.Security.Core.Domain;
using HR.Security.Core.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HR.Security.Core.Services.Users
{
    public partial interface IUserAccountService
    {
        Task<List<UserAccount>> GetAllAsync();
        Task<UserAccount> GetByNameAsync(string userName);
        Task<Tuple<AddUserResult, UserAccount>> AddUserAsync(string userName, string password);
        Task<List<UserAccount>> AddUserAccountsAsync(List<Tuple<string, string>> userAccounts);
        Task<Tuple<SignInStatus, UserAccount>> AuthenticateAsync(string userName, string password);

        /// <summary>
        /// 判断用户是否拥有角色
        /// </summary>
        /// <param name="userAccountId">用户ID</param>
        /// <param name="requireRoles">角色列表</param>
        /// <returns></returns>
        bool IsInRole(Guid userId, string[] requireRoles);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        Task<ResetPasswordResult> ResetPasswordAsync(Guid userId, string oldPassword, string newPassword);
    }
}
