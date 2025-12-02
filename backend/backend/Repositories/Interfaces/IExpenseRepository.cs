using backend.Core.Dto.GeneralDto;
using backend.Dto.Expense;
using backend.Model;
using backend.Model.Dto.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace backend.Core.Interfaces.IRepositories
{
    public interface IExpenseRepository
    {
        Task AddExpenses(Expense expense);

        Task<decimal> GetTotalExpense(Guid accountGroupId);

        Task<IEnumerable<Expense>> GetAllExpenses(Guid Id);

        Task<Expense> GetExpenseById(Guid Id);

        Task UpdateExpense(UpdateExpenseDto updateExpenseDto, Guid Id);

        Task DeleteExpense(Guid Id);
    }
}
