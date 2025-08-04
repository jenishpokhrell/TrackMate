using backend.Core.Dto.GeneralDto;
using backend.Dto.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IServices
{
    public interface IBudgetService
    {
        Task<GeneralServiceResponseDto> AddBudgetAsync(ClaimsPrincipal User, AddBudgetDto budgetDto);

        Task<GetBudgetDto> GetMyBudgetAsync(ClaimsPrincipal User);

        Task<GeneralServiceResponseDto> UpdateBudgetAsync(ClaimsPrincipal User, UpdateBudgetDto updateBudgetDto, Guid id);

        Task<GeneralServiceResponseDto> DeleteBudgetAsync(ClaimsPrincipal User, Guid id);
    }
}
