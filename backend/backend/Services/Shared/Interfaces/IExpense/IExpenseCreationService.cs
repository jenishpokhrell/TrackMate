using backend.Dto.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Services.Shared.Interfaces.IExpense
{
    public interface IExpenseCreationService
    {
        Task AddExpensesAsync(AddExpenseDto addExpenseDto);
    }
}
