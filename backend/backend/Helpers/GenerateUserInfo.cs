using backend.Dto.Auth;
using backend.Model;
using backend.Model.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Helpers
{
    public class GenerateUserInfo
    {
        public LoginInfo GenerateLoginInfo(ApplicationUser User, IList<string> roles)
        {
            return new LoginInfo
            {
                Id = User.Id,
                Email = User.Email,
                UserName = User.UserName,
                PhoneNumber = User.PhoneNumber,
                Roles = string.Join(", ", roles).ToLower()
            };
        }
        public UserInfo GenerateInfo(ApplicationUser User, IList<string> roles, Account account)
        {
            return new UserInfo
            {
                Id = User.Id,
                Email = User.Email,
                Username = User.UserName,
                Contact = User.PhoneNumber,
                Name = account.Name,
                Address = account.Address,
                Gender = account.Gender.ToString(),
                GroupName = account.AccountGroup?.Name,
                Roles = string.Join(", ", roles).ToLower()
            };
        }

        public UserInfoForAdmin GenerateInfoForAdmin(ApplicationUser User, IList<string> roles, Account account, AccountGroup accountGroup, AccountType
            accountType)
        {
            return new UserInfoForAdmin
            {
                Id = User.Id,
                Email = User.Email,
                Name = account.Name,
                UserName = User.UserName,
                Address = account.Address,
                PhoneNumber = User.PhoneNumber,
                Gender = account.Gender.ToString(),
                AccountGroupId = accountGroup.Id,
                AccountGroupName = accountGroup.Name,
                AdminUserId = accountGroup.AdminUserId,
                AccountCreatedAt = account.CreatedAt,
                AccountTypeId = accountType.Id,
                AccountType = accountType.Type,
                //Roles = string.Join(", ", roles).ToLower()
            };
        }
    }
}
