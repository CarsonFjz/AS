using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using HR.Security.Core.Data;
using HR.Security.Core.Domain;
using HR.Security.Core.Security;
using HR.Security.Core.Results;

namespace HR.Security.Core.Services.Users
{
    public partial class UserAccountService : IUserAccountService
    {        
        private readonly IRepository<UserAccount> _userAccountRepository;
        private readonly IRepository<RoleXUserAccount> _roleXUserAccountRepository;

        public UserAccountService(IRepository<UserAccount> userAccountRepository, IRepository<RoleXUserAccount> roleXUserAccountRepository)
        {
            this._userAccountRepository = userAccountRepository;
            this._roleXUserAccountRepository = roleXUserAccountRepository;
        }

        /// <summary>
        /// 验证用户名和密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<Tuple<SignInStatus, UserAccount>> AuthenticateAsync(string userName, string password)
        {
            #region Removed.

            ////1. 根据条件获取Token对象。
            //var authenticatedToken = await _tokenRepository.SingleOrDefaultAsync(x => x.AccessToken == token && !x.IsRevoked && x.ExpirationDate > DateTime.Now);

            //if(authenticatedToken != null)
            //{
            //    //2. 如果Token对象不为空，则为Token验证成功，建立Identity。
            //    IdentityDTO identityObj = new IdentityDTO();

            //    //3. 获取UserAccountXStaff对象。
            //    var uxs = await _userAccountXStaffRepository.IncludeSingleOrDefaultAsync(x => x.UserAccountID == authenticatedToken.UserAccountID, x => x.UserAccount, x => x.Staff);

            //    if (uxs != null)
            //    {
            //        if (uxs.UserAccount != null)
            //        {
            //            identityObj.UserID = uxs.UserAccount.ID;
            //            identityObj.UserName = uxs.UserAccount.UserName;
            //            identityObj.Status = uxs.UserAccount.Status;
            //        }
            //        if (uxs.Staff != null)
            //        {
            //            identityObj.StaffID = uxs.Staff.ID;
            //            identityObj.StaffNameCNLong = uxs.Staff.NameCNLong;
            //            identityObj.StaffNameENLong = uxs.Staff.NameENLong;
            //            identityObj.UnitID = uxs.Staff.UnitID;
            //            identityObj.UnitName = uxs.Staff.UnitName;
            //            identityObj.PostID = uxs.Staff.PostID;
            //            identityObj.PostName = uxs.Staff.PostName;
            //        }
            //    }
            //    else
            //    {
            //        //4. 防止UserAccountXStaff为空。
            //        var userAccount = await _userAccountRepository.GetByIdAsync(authenticatedToken.UserAccountID);

            //        if (userAccount != null)
            //        {
            //            identityObj.UserID = userAccount.ID;
            //            identityObj.UserName = userAccount.UserName;
            //            identityObj.Status = userAccount.Status;
            //        }
            //    }

            //    return identityObj;
            //}

            //return null;

            #endregion            

            SignInStatus status = SignInStatus.Failure;

            //1. 获取UserAccount对象。
            var userAccount = await _userAccountRepository.SingleOrDefaultAsync(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if(userAccount != null)
            {
                bool isLockedOut = userAccount.Status == UserAccount.STATUS_SUSPENDED;

                //2. 判断UserAccount对象是否被锁定。
                if (isLockedOut)
                {
                    status = SignInStatus.LocketOut;
                }
                else
                {
                    //3. 判断密码是否正确。                
                    bool isAuthenticated = Encryptor.CreatePasswordHash(password, userAccount.PasswordSalt) == userAccount.Password;
                    
                    status = isAuthenticated ? SignInStatus.Succeeded : SignInStatus.WrongPassword;
                }                
            }
                        
            return new Tuple<SignInStatus,UserAccount>(status, userAccount);
        }

        /// <summary>
        /// 添加多个用户
        /// </summary>
        /// <param name="userAccounts">用户列表</param>
        /// <returns></returns>
        public async Task<List<UserAccount>> AddUserAccountsAsync(List<Tuple<string, string>> userAccounts)
        {
            List<UserAccount> userAccountList = null;
            
            if(userAccounts != null && userAccounts.Count > 0)
            {
                userAccountList = new List<UserAccount>();

                foreach (var entity in userAccounts)
                {
                    string saltKey = Encryptor.CreateSaltKey(5);

                    UserAccount ua = new UserAccount()
                    {
                        CreatedBy = Guid.Empty,
                        CreatedDate = DateTime.Now,
                        LastModifiedBy = Guid.Empty,
                        LastModifiedDate = DateTime.Now,
                        UserName = entity.Item1,
                        Password = Encryptor.CreatePasswordHash(entity.Item2, saltKey),
                        PasswordSalt = saltKey,
                        Status = UserAccount.STATUS_ACTIVE
                    };

                    userAccountList.Add(ua);
                }

                await _userAccountRepository.InsertAsync(userAccountList);
            }

            return userAccountList;
        }

        /// <summary>
        /// 判断用户是否拥有角色
        /// </summary>
        /// <param name="userAccountId">用户ID</param>
        /// <param name="requireRoles">角色列表</param>
        /// <returns></returns>
        public bool IsInRole(Guid userId, string[] requireRoles)
        {
            if (requireRoles == null || requireRoles.Length == 0)
            {
                throw new ArgumentNullException("requireRoles");
            }

            int calculateCount = requireRoles.Length;

            var user = _userAccountRepository.GetById(userId);

            if(user != null)
            {
                List<RoleXUserAccount> rxuaList = _roleXUserAccountRepository.IncludeToList(x => x.UserAccountID == user.ID, x => x.Role);

                foreach (var rxua in rxuaList)
                {
                    if (calculateCount == 0)
                    {
                        break;
                    }

                    foreach (var requireRole in requireRoles)
                    {
                        if (rxua.Role.Name.Equals(requireRole, StringComparison.OrdinalIgnoreCase))
                        {
                            calculateCount--;

                            break;
                        }
                    }
                }
            }
            
            return calculateCount == 0;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<Tuple<AddUserResult, UserAccount>> AddUserAsync(string userName, string password)
        {
            AddUserResult result = AddUserResult.Failure;

            var user = await GetByNameAsync(userName);

            if(user != null)
            {
                result = AddUserResult.AlreadyExist;
            }
            else
            {
                string saltKey = Encryptor.CreateSaltKey(5);

                user = new UserAccount()
                {
                    UserName = userName,
                    Password = Encryptor.CreatePasswordHash(password, saltKey),
                    PasswordSalt = saltKey,
                    Status = UserAccount.STATUS_ACTIVE
                };

                await _userAccountRepository.InsertAsync(user);

                result = AddUserResult.Succeeded;
            }

            return new Tuple<AddUserResult,UserAccount>(result, user);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        public async Task<ResetPasswordResult> ResetPasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            ResetPasswordResult status = ResetPasswordResult.Failure;

            //1. 获取UserAccount对象。
            var userAccount = await _userAccountRepository.GetByIdAsync(userId);

            if(userAccount != null)
            {
                //2. 判断原密码是否正确。
                var isCorrectPassword = Encryptor.CreatePasswordHash(oldPassword, userAccount.PasswordSalt) == userAccount.Password;

                if(isCorrectPassword)
                {
                    //3. 若原密码正确则修改密码。
                    string saltKey = Encryptor.CreateSaltKey(5);
                    
                    userAccount.Password = Encryptor.CreatePasswordHash(newPassword, saltKey);
                    userAccount.PasswordSalt = saltKey;

                    await _userAccountRepository.UpdateAsync(userAccount);

                    status = ResetPasswordResult.Succeeded;
                }
                else
                {
                    status = ResetPasswordResult.WrongPassword;
                }
            }
            else
            {
                status = ResetPasswordResult.NotFound;
            }

            return status;
        }

        public async Task<List<UserAccount>> GetAllAsync()
        {
            return await _userAccountRepository.GetAllAsync();
        }

        public async Task<UserAccount> GetByNameAsync(string userName)
        {
            return await _userAccountRepository.SingleOrDefaultAsync(x => x.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
