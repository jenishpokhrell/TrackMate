using backend.Core.Dto.GeneralDto;
using backend.Dto.Expense;
using backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IServices
{
    public interface IExpenseService
    {
        Task<GeneralServiceResponseDto> AddExpensesAsync(ClaimsPrincipal User, AddExpenseDto addExpenseDto);

        Task<GetTotalExpensesDto> GetMyTotalExpensesAsync(ClaimsPrincipal User);
    }
}
