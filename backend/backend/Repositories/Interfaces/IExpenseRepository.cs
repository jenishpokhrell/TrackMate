using backend.Core.Dto.GeneralDto;
using backend.Dto.Expense;
using backend.Model;
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
    }
}
