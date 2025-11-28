using backend.Core.Dto.GeneralDto;
using backend.Dto.Expense;
using backend.Model;
using backend.Model.Dto.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IServices
{
    public interface IExpenseService
    {
        Task<GeneralServiceResponseDto> AddExpensesAsync(AddExpenseDto addExpenseDto);

        Task<GetTotalExpensesDto> GetMyTotalExpensesAsync(ClaimsPrincipal User);

        Task<GeneralServiceResponseDto> UpdateExpenseAsync(UpdateExpenseDto updateExpenseDto, Guid Id);

        Task<GeneralServiceResponseDto> DeleteExpenseAsync(Guid Id);
    }
}
