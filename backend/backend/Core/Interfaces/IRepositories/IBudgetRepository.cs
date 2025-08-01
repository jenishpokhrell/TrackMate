using backend.Core.Dto.GeneralDto;
using backend.Dto.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IRepositories
{
    public interface IBudgetRepository
    {
        Task<GeneralServiceResponseDto> AddBudget(ClaimsPrincipal User, AddBudgetDto addBudgetDto);
    }
}
