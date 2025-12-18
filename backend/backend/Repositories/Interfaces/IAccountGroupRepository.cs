using backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories.Interfaces
{
    public interface IAccountGroupRepository
    {
        Task<IEnumerable<AccountGroup>> GetAllAccountGroups();
        Task<AccountGroup> GetAccountGroupById(Guid accountGroupId);
    }
}
