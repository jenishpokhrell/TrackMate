using backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.Interfaces
{
    public interface IRoleManagementService
    {
        Task EnsureRoleExistsAsync(ApplicationUser User, string role);
    }
}
