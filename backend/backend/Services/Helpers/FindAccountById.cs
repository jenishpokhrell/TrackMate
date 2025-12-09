using backend.Core.Interfaces.IServices;
using backend.DataContext;
using backend.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Helpers
{
    public class FindAccountById : IFindAccountById
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserContextService _userContext;

        public FindAccountById(ApplicationDbContext context, IUserContextService userContext)
        {
            _context = context;
            _userContext = userContext;
        }
        public async Task<Account> GetAccountById(string UserId)
        {
            var loggedInUser = _userContext.GetCurrentLoggedInUserID();

            var account = await _context.Accounts.Include(a => a.AccountGroup).FirstOrDefaultAsync(a => a.UserId == loggedInUser);

            return account;

        }
    }
}
