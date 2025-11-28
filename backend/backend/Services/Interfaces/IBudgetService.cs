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
        Task<GeneralServiceResponseDto> AddBudgetAsync(AddBudgetDto budgetDto);

        Task<GetBudgetDto> GetMyBudgetAsync();

        Task<GetBudgetDto> GetMyRemainingBudgetAsync();

        Task<GeneralServiceResponseDto> UpdateBudgetAsync(UpdateBudgetDto updateBudgetDto, Guid id);

        Task<GeneralServiceResponseDto> DeleteBudgetAsync(ClaimsPrincipal User, Guid id);
    }
}
