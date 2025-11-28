using backend.Dto.Auth;
using backend.Model;
using backend.Model.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.Interfaces
{
    public interface IUserCreationService
    {
        Task<ApplicationUser> CreateUserAsync(RegisterUser userDto);
    }
}
