using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Helpers
{
    public interface IFindAccountGroupId
    {
        Task<Guid> FindAccountGroupIdAsync(string UserId);
    }
}
