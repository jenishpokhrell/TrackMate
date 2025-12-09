using backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Helpers
{
    public interface IFindAccountById
    {
        Task<Account> GetAccountById(string UserId);
    }
}
