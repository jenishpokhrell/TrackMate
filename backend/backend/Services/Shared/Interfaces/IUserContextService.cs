using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IServices
{
    public interface IUserContextService
    {
        string GetCurrentLoggedInUserID();

        string GetCurrentLoggedInUserUsername();

    }
}
