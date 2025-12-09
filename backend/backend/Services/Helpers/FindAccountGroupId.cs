using backend.DataContext;
using backend.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Helpers
{
    public class FindAccountGroupId : IFindAccountGroupId
    {
        private readonly ApplicationDbContext _context;

        public FindAccountGroupId(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> FindAccountGroupIdAsync(string UserId)
        {
            var accountGroupId = await _context.Accounts.Where(a => a.UserId == UserId).Select(a => a.AccountGroupId).FirstOrDefaultAsync();

            return accountGroupId;
        }
    }
}
