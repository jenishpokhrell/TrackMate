using backend.Dto.Auth;
using backend.Model;
using backend.Model.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task GetUserById(string UserId);
        Task<IEnumerable<ApplicationUser>> GetAllUsers();
        Task<IEnumerable<UserInfoForAdmin>> GetAllAccounts();
    }
}
